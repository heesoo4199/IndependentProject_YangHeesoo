#include <SPI.h>
#include <nRF24L01.h>
#include <RF24.h>
 
// SOURCE CODE (RADIO INITIALIZATION): HOWTOMECHATRONICS.COM AND ADAPTED BY NATHAN SHAO AND HEESOO YANG
RF24 radio(7, 8);
const byte designation[6] = "00001";
int radioNum = 1;  /* 0 for master, 1 for slave */
 
void setup()
{
  radio.begin();
 
  if(radioNum == 0)
  {
    Serial.begin(9600);
    radio.openWritingPipe(designation);
    radio.setPALevel(RF24_PA_MIN);
    radio.stopListening();
  }
  else
  {
    Serial.begin(9600);
    radio.openReadingPipe(0, designation);
    radio.setPALevel(RF24_PA_MIN);
    radio.startListening();
  }
 
}
 
 
void loop()
{
 
  if(radioNum == 0)
  {
    const char message[] = "Testing";
   
    /*if (Serial.available())
      {
        strcpy(message, Serial.read());
      }
      */
     
    if (!radio.write(&message,strlen(message)))
     {
       Serial.println(F("failed"));
     }
    
    Serial.println(message);
 
    delay(100);
  }
  else
  {
    String altitude = "";
    String X_Tilt = "";
    String Y_Tilt = "";
    String Clockwise_Yaw = "";
    String Counter_Yaw = "";
   
    if (radio.available())
    {
      char message[32] = "";
      radio.read(&message, sizeof(message));
      Serial.println(message);
 
      /*
      for(int i = 0; i < 4; i++)
      {
       altitude += message[i];
        X_Tilt += message[i+4];
        Y_Tilt += message[i+8];
        Clockwise_Yaw += message[i+12];
        Counter_Yaw += message[i+16];
      }
     
      if (!altitude.substring(0,2).equals("XX"))
      {
        Serial.print("altitude, ");
      }
      if (!X_Tilt.substring(0,2).equals("XX"))
      {
        Serial.print("X, ");
      }
      if (!Y_Tilt.substring(0,2).equals("XX"))
      {
        Serial.print("Y, ");
      }
      if (!Clockwise_Yaw.substring(0,2).equals("XX"))
      {
        Serial.print("Clockwise, ");
      }
      if (!Counter_Yaw.substring(0,2).equals("XX"))
      {
        Serial.print("Counter-Clockwise");
      }
      */
    }
  }
 
 
}
