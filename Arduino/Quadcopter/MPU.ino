#include <Wire.h>
#include <SPI.h>

uint32_t timer;

long accelX, accelY, accelZ; // Raw accel values
float gForceX, gForceY, gForceZ; // accel values in G
long gyroX, gyroY, gyroZ; // Raw gyro values
float rotX, rotY, rotZ; // gyro values in deg/s

float gyroOffset[3];

float posX, posY, posZ;
float angleX, angleY, angleZ;

void setup() 
{
  Serial.begin(9600);
  Wire.begin();
  setupMPU();
  calibrate();
  timer = micros();
}

void loop() 
{
  // put your main code here, to run repeatedly:
  complementary();
  printData();
}

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
  Serial.println(gForceZ);
}

void calibrate()
{
  for (int i = 0; i < 2000; i++)
  {
    gyroRead();
    gyroOffset[0] += rotX;
    gyroOffset[1] += rotY;
    gyroOffset[2] += rotZ;
  }
  gyroOffset[0] /= 2000;
  gyroOffset[1] /= 2000;
  gyroOffset[2] /= 2000;
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
void complementary()
{
  double dt = (double)(micros() - timer) / 1000000; //This line does three things: 1) stops the timer, 2)converts the timer's output to seconds from microseconds, 3)casts the value as a double saved to "dt".
  timer = micros();
  accelRead();
  gyroRead();
  adjust();
  float roll = atan2(accelY, accelZ) * 180 / 3.14159265358;
  float pitch = atan2(-accelX, accelZ) * 180 / 3.14159265358;
  angleX = 0.99 * (angleX + rotX * dt) + 0.01 * roll;
  angleY = 0.99 * (angleY + rotY * dt) + 0.01 * pitch;
  angleZ += rotZ * dt;
}
