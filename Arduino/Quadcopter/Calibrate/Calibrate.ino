#include <Servo.h>//Using servo library to control ESC
Servo esc1; //Creating a servo class with name as esc
Servo esc2; //Creating a servo class with name as esc
Servo esc3; //Creating a servo class with name as esc
Servo esc4; //Creating a servo class with name as esc
void setup()
{
  pinMode(1, OUTPUT);
esc1.attach(7); //Specify the esc signal pin,Here as D8
esc1.writeMicroseconds(1000); //initialize the signal to 1000
esc2.attach(2); //Specify the esc signal pin,Here as D8
esc2.writeMicroseconds(1000); //initialize the signal to 1000
esc3.attach(3); //Specify the esc signal pin,Here as D8
esc3.writeMicroseconds(1000); //initialize the signal to 1000
esc4.attach(4); //Specify the esc signal pin,Here as D8
esc4.writeMicroseconds(1000); //initialize the signal to 1000

Serial.begin(9600);
}
void loop()
{
int val; //Creating a variable val
val= analogRead(A0); //Read input from analog pin a0 and store in val
val= map(val, 0, 1023,1000,2000); //mapping val to minimum and maximum(Change if needed) 
esc1.writeMicroseconds(val); //using val as the signal to esc
esc2.writeMicroseconds(val);
esc3.writeMicroseconds(val);
esc4.writeMicroseconds(val);
Serial.println(val);
}
