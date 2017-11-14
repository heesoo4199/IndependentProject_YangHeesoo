#include <Servo.h> 

Servo ESC;

int pin = 0;  // potentiometer pin no.
int val;    // variable to read the value from the analog pin 

void setup() 
{ 
  ESC.attach(9);  // Use digital
} 

void loop() 
{ 
  val = analogRead(pin);            // reads the value of the potentiometer (value between 0 and 1023) 
  val = map(val, 0, 1023, 0, 179);     // scale it to use it with the servo (value between 0 and 180) 
  ESC.write(val);                  // sets the servo position according to the scaled value 
  delay(10); // 100Hz, does it work?
} //
