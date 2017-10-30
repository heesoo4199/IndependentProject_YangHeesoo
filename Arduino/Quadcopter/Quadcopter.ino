#include <PID_v1.h>

#include <Wire.h>
#include <SPI.h>
#include <nRF24L01.h>
#include <RF24.h>

// MPU-6050 constants
float rotX, rotY, rotZ;
float accX, accY, accZ;
float angleX, angleY, angleZ;

// nRF24L01 constants
RF24 radio(7, 8);
const byte radio_address[6] = "00001";

// PID controller constants
double setpoint, input, output;
double kP = 1, kI = 1, kD = 1;
PID PIDController(input, output, setpoint, kP, kI, kD, DIRECT);

void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
  // nRF24L01 setup
  radio.begin();
  radio.openReadingPipe(0, address);
  radio.setPALevel(RF24_PA_MIN);
  radio.startListening();
  // MPU-6050 setup
  Wire.begin();
  mpuInit(); // Initializes the MPU
  angleX = 0;
  angleY = 0;
  angleZ = 0;
  // PID setup
  // Input = sensor, so gyro/accel reading
  PIDController.SetMode(AUTOMATIC);
}

void loop() {
  // put your main code here, to run repeatedly:
  gyroRead();
  angleX += rotX;
  angleY += rotY;
  angleZ += rotZ;
  Serial.print(angleX);
  if (radio.available()) {
    // Do radio requests/processing here
  }
  delay(100);
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

