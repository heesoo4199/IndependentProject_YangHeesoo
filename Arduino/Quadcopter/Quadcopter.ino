#include <Wire.h>
#include <SPI.h>
#include <nRF24L01.h>
#include <RF24.h>

// Arduino variables
bool started;
long timer; // tracks time in microseconds.

// MPU-6050 constants
float gyroX, gyroY, gyroZ; // Angular velocity (deg/s)
float accX, accY, accZ; // Acceleration (g)

float angleX, angleY, angleZ;

// nRF24L01 constants//
RF24 radio(7, 8); // Radio connection pins 7 & 8
const byte radio_address[6] = "00001"; // Must match with transmitter

Servo esc1, esc2, esc3, esc4; // 1 FR, 3 RL (CCW) | 2 RR, 4 FL (CW)
int esc1_val, esc2_val, esc3_val, esc4_val; // Pulses for respective ESC in microseconds

float pid_p = 1.3;
float pid_i = 0.04;
float pid_d = 18.0;
float pid_yaw_p = 4.0;
float pid_yaw_i = 0.02;
float pid_yaw_d = 0;
int pid_max = 200;

float pid_mem_roll = 0.0;
float pid_mem_pitch = 0.0;
float pid_mem_yaw = 0.0;

float pid_last_err_roll = 0.0;
float pid_last_err_pitch = 0.0;
float pid_last_err_yaw = 0.0;

void setup() {
  Serial.begin(9600);
  // nRF24L01 setup
  radio.begin();
  radio.openReadingPipe(0, address);
  radio.setPALevel(RF24_PA_MIN);
  radio.startListening();
  // MPU-6050 setup
  Wire.begin();
  mpuInit();
  // Connect ESCs
  esc1.attach(1);
  esc1.writeMicroseconds(1000);
  esc2.attach(2);
  esc2.writeMicroseconds(1000);
  esc3.attach(3);
  esc3.writeMicroseconds(1000);
  esc4.attach(4);
  esc4.writeMicroseconds(1000);
}

void loop() {
  timer = micros();
  // Read and update gyroscope and accelerometer values
  gyroRead();
  accelRead();
  // Radio receiver processing
  if (radio.available()) {
    
  }
  if (started) 
  {
    esc1.writeMicroseconds(esc1_val);
    esc2.writeMicroseconds(esc2_val);
    esc3.writeMicroseconds(esc3_val);
    esc4.writeMicroseconds(esc4_val);
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
    gyroX = (Wire.read()<<8|Wire.read()) / 131.0; // Divid raw value by 131 (LSB per degree)
    gyroY = (Wire.read()<<8|Wire.read()) / 131.0;
    gyroZ = (Wire.read()<<8|Wire.read()) / 131.0;
  }
}

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

// Adjust ESC output based on user input.
void PID(int throttle, int roll, int pitch, int yaw) { 
  if (started) 
  {
    if (throttle > 1800) 
    {
      throttle = 1800; // We need some room to keep full control at full throttle. 
    }
    // Calculate PID setpoints based on input
    // Say Joystick returns x (sideways), y (forwards/backwards), and z (rotation)
    // And the map for the values is 1000 to 2000, where 1500 is the neutral position
    // What should the max tilt be? Let's say 15 degrees.
    // Then, the error in degrees is:
    float err_roll = 15.0 / 500.0 * (roll - 1500) - angleX;
    float err_pitch = 15.0 / 500.0 * (pitch - 1500) - angleY;
    // For Z, use angular velocity, not position. Say, 30 deg/s
    float err_yaw = 30.0 / 500.0 * (yaw - 1500);

    // Accumulate the integral
    pid_mem_roll += pid_i * err_roll;
    pid_mem_pitch += pid_i * err_pitch;
    pid_mem_yaw += pid_yaw_i * err_yaw;
    // Don't want values over the max pid output being aggregated to integral
    if (pid_mem_roll > pid_max)
      pid_mem_roll = pid_max
    else if (pid_mem_roll < -pid_max)
      pid_mem_roll = -pid_max;
    if (pid_mem_pitch > pid_max)
      pid_mem_pitch = pid_max
    else if (pid_mem_pitch < -pid_max)
      pid_mem_pitch = -pid_max;
    if (pid_mem_yaw > pid_max)
      pid_mem_yaw = pid_max
    else if (pid_mem_yaw < -pid_max)
      pid_mem_yaw = -pid_max;

    // Store err for use in next loop
    pid_last_err_roll = err_roll;
    pid_last_err_pitch = err_pitch;
    pid_last_err_yaw = err_yaw;

    pid_output_roll = pid_p * err_roll + pid_mem_roll + pid_d * (err_roll - pid_last_err_roll);
    pid_output_pitch = pid_p * err_pitch + pid_mem_pitch + pid_d * (err_pitch - pid_last_err_pitch);
    pid_output_yaw = pid_yaw_p * err_yaw + pid_mem_yaw + pid_yaw_d * (err_yaw - pid_last_err_yaw);

    // Apply PID output to ESCs
    esc1_val = throttle - pid_output_pitch + pid_output_roll - pid_output_yaw; //(front-right - CCW)
    esc2_val = throttle + pid_output_pitch + pid_output_roll + pid_output_yaw; //(rear-right - CW)
    esc3_val = throttle + pid_output_pitch - pid_output_roll - pid_output_yaw; //(rear-left - CCW)
    esc4_val = throttle - pid_output_pitch - pid_output_roll + pid_output_yaw; //(front-left - CW)
    
    // Limit operating range of ESCs to 1100us to 2000us while flight mode is active
    if (esc1_val < 1100) 
      esc1_val = 1100;                                         
    if (esc2_val < 1100) 
      esc2_val = 1100;                                         
    if (esc3_val < 1000) 
      esc3_val = 1100;                                        
    if (esc4_val < 1100) 
      esc4_val = 1100; 
    if (esc1_val > 2000)
      esc1_val = 2000;
    if (esc2_val > 2000)
      esc2_val = 2000;
    if (esc3_val > 2000)
      esc3_val = 2000;
    if (esc4_val > 2000)
      esc4_val = 2000;                                      
  } 
  else // Ensure motors are off when flight mode is not active
  {
    esc_1 = 1000;                                                      
    esc_2 = 1000;                                                          
    esc_3 = 1000;                                                           
    esc_4 = 1000;                                                        
  }
}

