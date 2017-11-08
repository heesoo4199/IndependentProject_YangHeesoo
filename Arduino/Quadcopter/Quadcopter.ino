#include <Wire.h>
#include <SPI.h>
#include <nRF24L01.h>
#include <RF24.h>
#include <PID_v1.h>

// Arduino variables
long timer; // tracks time in microseconds.

// Drone coordinate system
float x, y, z; // x, y, z (meters)
float roll, pitch, yaw; // roll & pitch = 0 if parallel to ground. yaw = 0 at start (deg)

// MPU-6050 constants
float rotX, rotY, rotZ; // Angular velocity (deg/s)
float accX, accY, accZ; // Acceleration (g)

// nRF24L01 constants
RF24 radio(7, 8); // Radio connection pins 7 & 8
const byte radio_address[6] = "00001"; // Must match with transmitter

// PID controller constants
double setpoint, input, output;
double kP = 1, kI = 1, kD = 1;
PID PIDController(input, output, setpoint, kP, kI, kD, DIRECT);

void setup() {
  Serial.begin(9600);
  // nRF24L01 setup
  radio.begin();
  radio.openReadingPipe(0, address);
  radio.setPALevel(RF24_PA_MIN);
  radio.startListening();
  // MPU-6050 setup
  Wire.begin();
  mpuInit(); // Initializes the MPU
  // PID setup
  // Input = sensor, so gyro/accel reading
  PIDController.SetMode(AUTOMATIC);

  // Coordinates initialization
  x = 0;
  y = 0;
  z = 0;
  yaw = 0;

  // Set up control timer
  timer = micros();
}

void loop() {
  timer = micros();
  // Read and update gyroscope and accelerometer values
  gyroRead();
  accelRead();
  // Radio receiver processing
  if (radio.available()) {
    
  }
}

// Disables sleep mode, and sets sensitivity for the gyro and accelerometer.
// https://www.invensense.com/wp-content/uploads/2015/02/MPU-6000-Register-Map1.pdf
void mpuInit() {
  // Disable sleep mode
  Wire.beginTransmission(0b1101000); // MPU access register
  Wire.write(0x6B); // 6B register for power management
  Wire.write(0b00000000); // 2nd bit = 0 for sleep off
  Wire.endTransmission();
  // Set gyro sensitivity
  Wire.beginTransmission(0b1101000);
  Wire.write(0x1B);
  Wire.write(0b00000000);
  Wire.endTransmission();
  // Set accelerometer sensitivity
  Wire.beginTransmission(0b1101000);
  Wire.write(0x1C);
  Wire.write(0b00000000);
  Wire.endTransmission();
}

// Sets rotX, rotY, and rotZ to respective gyro values (in degrees per second)
void gyroRead() {
  Wire.beginTransmission(0b11010000);
  Wire.write(0x43);
  Wire.endTransmission();
  Wire.requestFrom(0b11010000, 6); // Requests values from address 43 to 48
  while (Wire.available() < 6) { 
    rotX = (Wire.read()<<8|Wire.read()) / 131.0; // Divid raw value by 131 (LSB per degree)
    rotY = (Wire.read()<<8|Wire.read()) / 131.0;
    rotZ = (Wire.read()<<8|Wire.read()) / 131.0;
  }
}
// com
// Sets accX, accY, and accZ to respective accelerometer values (in Gs)
void accelRead() {
  Wire.beginTransmission(0b11010000);
  Wire.write(0x3B);
  Wire.endTransmission();
  Wire.requestFrom(0b11010000, 6); // Requests values from address 3B to 40
  while (Wire.available() < 6) { 
    accX = (Wire.read()<<8|Wire.read()) / 16384.0; // Divid raw value by 16384 (LSB per g)
    accY = (Wire.read()<<8|Wire.read()) / 16384.0;
    accZ = (Wire.read()<<8|Wire.read()) / 16384.0;
  }
}

void gyroCalibrate() {
  //wip
}

void PID(int throttle) {

  if (/*k*/) {                                                          //The motors are started.
    if (throttle > 1800) {
      throttle = 1800;                                   //We need some room to keep full control at full throttle. 
    }
    esc1 = throttle - pid_output_pitch + pid_output_roll - pid_output_yaw; //(front-right - CCW)
    esc2 = throttle + pid_output_pitch + pid_output_roll + pid_output_yaw; //(rear-right - CW)
    esc3 = throttle + pid_output_pitch - pid_output_roll - pid_output_yaw; //(rear-left - CCW)
    esc4 = throttle - pid_output_pitch - pid_output_roll + pid_output_yaw; //(front-left - CW)

    if (esc1 < 1100) 
      esc_1 = 1100;                                         
    if (esc2 < 1100) 
      esc_2 = 1100;                                         
    if (esc3 < 1100) 
      esc_3 = 1100;                                        
    if (esc4 < 1100) 
      esc_4 = 1100;                                         
  }
  else{
    esc_1 = 1000;                                                      
    esc_2 = 1000;                                                          
    esc_3 = 1000;                                                           
    esc_4 = 1000;                                                        
  }
}

