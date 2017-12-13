#include <Wire.h>
#include <SPI.h>
#include <nRF24L01.h>
#include <RF24.h>
#include <Servo.h>

// Arduino variables
uint32_t timer;
bool started = false;

long accelX, accelY, accelZ; // Raw accel values
float gForceX, gForceY, gForceZ; // accel values in G
long gyroX, gyroY, gyroZ; // Raw gyro values
float rotX, rotY, rotZ; // gyro values in deg/s

double gyroOffset[3];

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
  radio.openReadingPipe(0, radio_address);
  radio.setPALevel(RF24_PA_MIN);
  radio.startListening();
  // MPU-6050 setup
  Wire.begin();
  setupMPU();
  calibrate();
  timer = micros();
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
  float dt = (float) (micros() - timer) / 1000000;
  timer = micros();
  complementary(dt);
  //printData();
  // Radio receiver processing
  if (radio.available()) {
    // Take throttle value for now, and store it to throttle
    //int throttle = radio.get something
    if (started) 
    {
      PID(throttle, 0, 0, 0);
      esc1.writeMicroseconds(esc1_val);
      esc2.writeMicroseconds(esc2_val);
      esc3.writeMicroseconds(esc3_val);
      esc4.writeMicroseconds(esc4_val);
    }  
    printData();
  }
}

// Disables sleep mode, and sets sensitivity for the gyro and accelerometer.
// https://www.invensense.com/wp-content/uploads/2015/02/MPU-6000-Register-Map1.pdf
void setupMPU()
{
  Wire.beginTransmission(0b1101000); //This is the I2C address of the MPU (b1101000/b1101001 for AC0 low/high datasheet sec. 9.2)
  Wire.write(0x6B); //Accessing the register 6B - Power Management (Sec. 4.28)
  Wire.write(0b00000000); //Setting SLEEP register to 0. (Required; see Note on p. 9)
  Wire.endTransmission();
  Wire.beginTransmission(0b1101000); //I2C address of the MPU
  Wire.write(0x1B); //Accessing the register 1B - Gyroscope Configuration (Sec. 4.4)
  Wire.write(0x00000000); //Setting the gyro to full scale +/- 250deg./s
  Wire.endTransmission();
  Wire.beginTransmission(0b1101000); //I2C address of the MPU
  Wire.write(0x1C); //Accessing the register 1C - Acccelerometer Configuration (Sec. 4.5)
  Wire.write(0b00000000); //Setting the accel to +/- 2g
  Wire.endTransmission();
}

// Sets accX, accY, and accZ to respective accelerometer values (in Gs)
void accelRead() 
{
  Wire.beginTransmission(0b1101000);
  Wire.write(0x3B);
  Wire.endTransmission();
  Wire.requestFrom(0b1101000, 6); // Requests values from address 3B to 40
  while(Wire.available() < 6); 
  accelX = Wire.read()<<8|Wire.read(); //Store first two bytes into accelX
  accelY = Wire.read()<<8|Wire.read(); //Store middle two bytes into accelY
  accelZ = Wire.read()<<8|Wire.read(); //Store last two bytes into accelZ
  gForceX = accelX / 16384.0;
  gForceY = accelY / 16384.0;
  gForceZ = accelZ / 16384.0;
}

// Sets rotX, rotY, and rotZ to respective gyro values (in degrees per second)
void gyroRead() 
{
  Wire.beginTransmission(0b1101000);
  Wire.write(0x43);
  Wire.endTransmission();
  Wire.requestFrom(0b1101000, 6); // Requests values from address 43 to 48
  while (Wire.available() < 6); 
  gyroX = Wire.read()<<8|Wire.read();
  gyroY = Wire.read()<<8|Wire.read();
  gyroZ = Wire.read()<<8|Wire.read();
  rotX = gyroX / 131.0;
  rotY = gyroY / 131.0;
  rotZ = gyroZ / 131.0;
}

void printData() {
  Serial.print("Gyro (deg)");
  Serial.print(" X=");
  Serial.print(angleX);
  Serial.print(" Y=");
  Serial.print(angleY);
  Serial.print(" Z=");
  Serial.print(angleZ);
  Serial.print(" Accel (g)");
  Serial.print(" X=");
  Serial.print(gForceX);
  Serial.print(" Y=");
  Serial.print(gForceY);
  Serial.print(" Z=");
  Serial.print(gForceZ);
  Serial.print(", ESC1: ");
  Serial.print(esc1_val);
  Serial.print(", ESC2: ");
  Serial.print(esc2_val);
  Serial.print(", ESC3: ");
  Serial.print(esc3_val);
  Serial.print(", ESC4: ");
  Serial.println(esc4_val);
}

void calibrate()
{
  int s = 2000;
  for (int i = 0; i < s; i++)
  {
    gyroRead();
    gyroOffset[0] += rotX;
    gyroOffset[1] += rotY;
    gyroOffset[2] += rotZ;
    delayMicroseconds(1000);
  }
  gyroOffset[0] /= s;
  gyroOffset[1] /= s;
  gyroOffset[2] /= s;
  Serial.println(gyroOffset[0]);
  Serial.println(gyroOffset[1]);
  Serial.println(gyroOffset[2]);
}

void adjust()
{
  rotX -= gyroOffset[0];
  rotY -= gyroOffset[1];
  rotZ -= gyroOffset[2];
}

// angle = 0.98 *(angle+gyro*dt) + 0.02*acc
void complementary(float dt)
{
  accelRead();
  gyroRead();
  adjust();
  float roll = atan2(accelY, accelZ) * 180 / 3.14159265358;
  float pitch = atan2(-accelX, accelZ) * 180 / 3.14159265358;
  angleX = 0.99 * (angleX + rotX * dt) + 0.01 * roll;
  angleY = 0.99 * (angleY + rotY * dt) + 0.01 * pitch;
  angleZ += rotZ * dt;
}

// Adjust ESC output based on user input.
void PID(int throttle, int roll, int pitch, int yaw) { 
  if (true) 
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
    // For Z, use angular velocity, not position. Max of 30 deg/s
    float err_yaw = 30.0 / 500.0 * (yaw - 1500);

    // Accumulate the integral
    pid_mem_roll += pid_i * err_roll;
    pid_mem_pitch += pid_i * err_pitch;
    pid_mem_yaw += pid_yaw_i * err_yaw;
    
    // Don't want values over the max pid output being aggregated to integral
    if (pid_mem_roll > pid_max)
      pid_mem_roll = pid_max;
    else if (pid_mem_roll < -pid_max)
      pid_mem_roll = -pid_max;
    if (pid_mem_pitch > pid_max)
      pid_mem_pitch = pid_max;
    else if (pid_mem_pitch < -pid_max)
      pid_mem_pitch = -pid_max;
    if (pid_mem_yaw > pid_max)
      pid_mem_yaw = pid_max;
    else if (pid_mem_yaw < -pid_max)
      pid_mem_yaw = -pid_max;

    // Store err for use in next loop
    pid_last_err_roll = err_roll;
    pid_last_err_pitch = err_pitch;
    pid_last_err_yaw = err_yaw;

    // Calculate PID output
    float pid_output_roll = pid_p * err_roll + pid_mem_roll + pid_d * (err_roll - pid_last_err_roll);
    float pid_output_pitch = pid_p * err_pitch + pid_mem_pitch + pid_d * (err_pitch - pid_last_err_pitch);
    float pid_output_yaw = pid_yaw_p * err_yaw + pid_mem_yaw + pid_yaw_d * (err_yaw - pid_last_err_yaw);

    // Limit PID output to specified value (pid_max);
    if (pid_output_roll > pid_max)
      pid_output_roll = pid_max;
    else if (pid output_roll < -pid_max)
      pid_output_roll = -pid_max;
    if (pid_output_pitch > pid_max)
      pid_output_pitch = pid_max;
    else if (pid output_pitch < -pid_max)
      pid_output_pitch = -pid_max;
    if (pid_output_yaw > pid_max)
      pid_output_yaw = pid_max;
    else if (pid output_yaw < -pid_max)
      pid_output_yaw = -pid_max; 
      
    // Apply PID output to ESCs
    esc1_val = throttle - pid_output_pitch + pid_output_roll;// - pid_output_yaw; //(front-right - CCW)
    esc2_val = throttle + pid_output_pitch + pid_output_roll;// + pid_output_yaw; //(rear-right - CW)
    esc3_val = throttle + pid_output_pitch - pid_output_roll;// - pid_output_yaw; //(rear-left - CCW)
    esc4_val = throttle - pid_output_pitch - pid_output_roll;// + pid_output_yaw; //(front-left - CW)
     
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

    // Print stuff
    Serial.print("Gyro (deg)");
    Serial.print(" X=");
    Serial.print(angleX);
    Serial.print(" Y=");
    Serial.print(angleY);
    Serial.print(" Z=");
    Serial.print(angleZ);
    Serial.print(" Accel (g)");
    Serial.print(" X=");
    Serial.print(gForceX);
    Serial.print(" Y=");
    Serial.print(gForceY);
    Serial.print(" Z=");
    Serial.print(gForceZ);
    Serial.print(", ESC1: ");
    Serial.print(esc1_val);
    Serial.print(", ESC2: ");
    Serial.print(esc2_val);
    Serial.print(", ESC3: ");
    Serial.print(esc3_val);
    Serial.print(", ESC4: ");
    Serial.print(esc4_val);
    Serial.print(", roll: ");
    Serial.print(pid_output_roll);
    Serial.print(", pitch: ");
    Serial.print(pid_output_pitch);
    Serial.print(", yaw: ");
    Serial.println(pid_output_yaw); 
  } 
  else // Ensure motors are off when flight mode is not active
  {
    esc1_val = 1000;                                                      
    esc2_val = 1000;                                                          
    esc3_val = 1000;                                                           
    esc4_val = 1000;                                                        
  }
}

