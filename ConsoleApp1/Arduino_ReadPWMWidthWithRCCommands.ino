//Arduino code for Arm-Trigger test bench.
//Uses Arduino MEGA
//all unmarked connectors are number left to right
//Arduino PIN  ---- Other Devices pin

// 2  (PWM)   ----  SmartAir Servo Signal
// GND        ----  SmartAir Servo GND
// 4  (PWM)   ----  SmartAir ARM PIN
// 13 (PWM)   ----  SmartAir Trigger PIN
// 6  (PWM)   ----  SmartAir Relay EN1
// GND        ----  SmartAir Servo GND
// 5V         ----  Relay 5V
// AO         ----  Between two voltage devider resistor (check voltage is lower than 5V at trigger)
String inputString = "";         // a String to hold incoming data
String inputString2 = "";

boolean stringComplete = false;  // whether the string is complete
boolean stringComplete2 = false;
boolean wasInLow = true;
boolean printPulseWidth = false;

#define CHANNEL_1_PIN 3
#define CHANNEL_2_PIN 2
volatile unsigned long start_timer;
volatile unsigned long pulse_width;
volatile unsigned long Zero_Changes_Counter;
volatile unsigned long Previous_Zero_Changes_Counter;
volatile float Previous_Voltage;
float multi = 1.6;
int i;
int DeathCounter;

boolean PWMwasInLow = true;
boolean PWMwasInHigh = true;
boolean PWMReset = true;

void calcSignal2() 
{
    //Serial.println("c");
    if( (digitalRead(CHANNEL_2_PIN) == HIGH) && (PWMwasInLow == true))
    {
        start_timer = micros();
        //Serial.println("Motor Signal High");
        PWMwasInLow = false;
        PWMwasInHigh = true;
    }
    //otherwise, the pin has gone LOW
    if ( (digitalRead(CHANNEL_2_PIN) == LOW) && (PWMwasInHigh == true) )
    {
        //Serial.print("a");
        pulse_width = (( (volatile long)micros() - start_timer)/10)*10;
        //if( (digitalRead(CHANNEL_1_PIN) == HIGH)&& (printPulseWidth == true))
        //{
          Serial.print("PWM Pulse Width: " );
          Serial.println(pulse_width);
          DeathCounter++;
          if (DeathCounter==1000)
          {
            DeathCounter = 0;
            detachInterrupt(digitalPinToInterrupt(CHANNEL_2_PIN)); 
          }
          //printPulseWidth = false;
        //}
        //Serial.println("Motor Signal Low");        
        PWMwasInLow = false;
        PWMwasInHigh = false;
        PWMReset = true;
    }

    if ( (digitalRead(CHANNEL_2_PIN) == LOW) && (PWMReset == true) )
    {
        
        PWMwasInLow = true;
        PWMwasInHigh = false;
    }
}

void setup() {
  // initialize serial:
  pinMode (4 , OUTPUT); // arming pin
  pinMode (13 , OUTPUT); // Trigger pin
  pinMode (6 , OUTPUT); // Relay EN pin
  digitalWrite(4, LOW);
  digitalWrite(13, LOW);
  digitalWrite(6, LOW);
  PWMwasInLow = false;
  PWMwasInHigh = false;
  PWMReset = true;
  Serial.begin(115200);
  // reserve 200 bytes for the inputString:
  inputString2.reserve(200);

  //attachInterrupt(digitalPinToInterrupt(CHANNEL_2_PIN), calcSignal2, CHANGE);
}

void loop() {
LowPWM();
}
void LowPWM()
{
  digitalWrite(13, HIGH);
  digitalWrite(4, HIGH);
  delay(1);
  digitalWrite(13, LOW);
  digitalWrite(4, LOW);
  delay(19);
}

void serialEvent()
{
  String TmpStr = "";
  if (Serial.available())
  {
    //start_timer = micros();
    TmpStr = "";
    TmpStr = Serial.readString();
    Serial.println("NewLine");
    Serial.println(TmpStr);
    Serial.println("ENdLine");

    if (TmpStr.indexOf("ARM") >= 0)
    {
      Zero_Changes_Counter = 0;
      Previous_Zero_Changes_Counter = 0;
      Zero_Changes_Counter = 0;
      Serial.println("exec ARM");
      for (i = 0; i < 401; i++)
      {
        if (i < 200)
        {
          LowPWM();
          //Serial.println("Low");

        }
        else if (i >= 200 & i < 400)
        {
          HighPWMARM();
          //Serial.println("High");
        }
        else if (i >= 400)
        {
          if (i == 400)
            Serial.println("ARMEnded");
          LowPWM();
          //Serial.println("Low");
        }
      }
    }

    if (TmpStr.indexOf("FIRE") >= 0)
    {
      Zero_Changes_Counter = 0;
      Previous_Zero_Changes_Counter = 0;
      Zero_Changes_Counter = 0;
      Serial.println("exec Fire");
      for (i = 0; i < 401; i++)
      {
        if (i < 200)
        {
          LowPWM();
          //Serial.println("Low");

        }
        else if (i >= 200 & i < 400)
        {
          HighPWMTRIGGER();
          //Serial.println("High");
        }
        else if (i >= 400)
        {
          if (i == 400)
            Serial.println("FIREEnded");
          LowPWM();
          //Serial.println("Low");
        }
      }
    }
    if (TmpStr.indexOf("CLR") >= 0)
    {
      Zero_Changes_Counter = 0;
      Previous_Zero_Changes_Counter = 0;
      Zero_Changes_Counter = 0;
    }

    if (TmpStr.indexOf("Mode") >= 0)
    {
      Zero_Changes_Counter = 0;
      Previous_Zero_Changes_Counter = 0;
      Zero_Changes_Counter = 0;
      for (int i = 0; i < 160 * multi + 20 ; i++)
      {
        if (i < 20 * multi)
        {
          //if (i == 1)
          //Serial.println("Low1");
          LowPWM();
        }
        else if ((i >= 20 * multi) & (i < 40 * multi))
        {
          //if (i == 20 * multi)
          //Serial.println("High1");
          HighPWMARM();
        }//One
        else if ((i >= 40 * multi) & (i < 60 * multi))
        {
          //if (i == 40 * multi)
          //Serial.println("Low2");
          LowPWM();
        }
        else if ((i >= 60 * multi) & (i < 80 * multi))
        {
          //if (i == 60 * multi)
          //Serial.println("High2");
          HighPWMARM();
        }//Two
        else if ((i >= 80 * multi) & (i < 100 * multi))
        {
          //if (i == 80 * multi)
          //Serial.println("Low3");
          LowPWM();
        }
        else if ((i >= 100 * multi) & (i < 120 * multi))
        {
          //if (i == 100 * multi)
          //Serial.println("High3");
          HighPWMARM();
        }//Three
        else if ((i >= 120 * multi) & (i < 140 * multi))
        {
          //if (i == 120 * multi)
          //Serial.println("Low4");
          LowPWM();
        }
        else if ((i >= 140 * multi) & (i < 160 * multi))
        {
          //if (i == 140 * multi)
          //Serial.println("High4");
          HighPWMARM();
        }//Four
        //else if ((i >= 160 * multi))
        //{
        //if (i == 160 * multi)
        //Serial.println("LowFinal");
        //LowPWM();
        //}
      }
      Serial.println("Mode Sequence Ended");
    }

    if (TmpStr.indexOf("XBTLow") >= 0)
    {

      LowPWM();
      digitalWrite(5, LOW);
      delay(10);
      Serial.println("exec XBT Low");
    }
    if (TmpStr.indexOf("XBTHigh") >= 0)
    {

      LowPWM();
      digitalWrite(5, HIGH);
      delay(10);
      Serial.println("exec XBT High");
    }

    if (TmpStr.indexOf("PWRUP") >= 0)
    {
      Serial.println("exec PWRUP");
      detachInterrupt(digitalPinToInterrupt(CHANNEL_2_PIN));
      Zero_Changes_Counter = 0;
      Previous_Zero_Changes_Counter = 0;
      Zero_Changes_Counter = 0;
      
      //LowPWM();
      digitalWrite(6, HIGH);
      Serial.println("PWRUPEnded");
    }
    if (TmpStr.indexOf("PWRDWN") >= 0)
    {
      Serial.println("exec PWRDWN");
      detachInterrupt(digitalPinToInterrupt(CHANNEL_2_PIN));
      Zero_Changes_Counter = 0;
      Previous_Zero_Changes_Counter = 0;
      Zero_Changes_Counter = 0;
      
      //LowPWM();
      digitalWrite(6, LOW);
      delay(2000);
      Serial.println("PWRDWNEnded");
    }

    if (TmpStr.indexOf("TSTVLTG") >= 0)
    {
      Serial.println("exec TSTVLTG");
      detachInterrupt(digitalPinToInterrupt(CHANNEL_2_PIN)); 
      delay(2000);
      //Serial.flush();
      Zero_Changes_Counter = 0;
      Previous_Zero_Changes_Counter = 0;
      Zero_Changes_Counter = 0;
      
      //LowPWM();

      voltageMeasurement();
      Serial.println("TSTVLTGEnded");
      delay(2000);

    }

    if (TmpStr.indexOf("PWMREAD") >= 0)
    {
      Serial.println("exec PWMREAD");
      Zero_Changes_Counter = 0;
      Previous_Zero_Changes_Counter = 0;
      Zero_Changes_Counter = 0;
      
      //LowPWM();
      delay(2000);
      Serial.println("PWMREADEnded");
      attachInterrupt(digitalPinToInterrupt(CHANNEL_2_PIN), calcSignal2, CHANGE);
    }

    if (TmpStr.indexOf("PWMREADStop") >= 0)
    {
      Serial.println("exec PWMREADStop");
      detachInterrupt(digitalPinToInterrupt(CHANNEL_2_PIN)); 
      Zero_Changes_Counter = 0;
      Previous_Zero_Changes_Counter = 0;
      Zero_Changes_Counter = 0;
      
      //LowPWM();
      delay(2000);
      Serial.println("PWMREADStopEnded");
    }
  }
}

//High PWM in Arm channel Only
void HighPWMARM()
{
  /*digitalWrite(4, HIGH);
    digitalWrite(13, HIGH);
    delayMicroseconds(1000);
    digitalWrite(13, LOW);
    delayMicroseconds(1000);
    digitalWrite(4, LOW);
    delayMicroseconds(2000);
    delayMicroseconds(2000);
    delayMicroseconds(2000);
    delayMicroseconds(2000);
    delayMicroseconds(2000);
    delayMicroseconds(2000);
    delayMicroseconds(2000);
    delayMicroseconds(2000);
    delayMicroseconds(1800);
    /*digitalWrite(13, HIGH);
    digitalWrite(4, HIGH);
    delay(1);
    digitalWrite(13, LOW);
    delay(1);
    //digitalWrite(13, LOW);
    digitalWrite(4, LOW);
    delay(18);
  */
  digitalWrite(13, HIGH);
  digitalWrite(4, HIGH);
  delay(1);
  digitalWrite(13, LOW);
  delay(17);
  //digitalWrite(13, LOW);
  digitalWrite(4, LOW);
  delay(2);
}

//High PWM in Trigger channel Only
void HighPWMTRIGGER()
{
  /*digitalWrite(4, HIGH);
    digitalWrite(13, HIGH);
    delayMicroseconds(1000);
    digitalWrite(4, LOW);
    delayMicroseconds(1000);
    digitalWrite(13, LOW);
    delayMicroseconds(2000);
    delayMicroseconds(2000);
    delayMicroseconds(2000);
    delayMicroseconds(2000);
    delayMicroseconds(2000);
    delayMicroseconds(2000);
    delayMicroseconds(2000);
    delayMicroseconds(2000);
    delayMicroseconds(2000);*/
  digitalWrite(13, HIGH);
  digitalWrite(4, HIGH);
  delay(1);
  digitalWrite(4, LOW);
  delay(17);
  digitalWrite(13, LOW);
  //digitalWrite(4, LOW);
  delay(2);
}

//High PWM in Trigger channel Only
void HighPWMBothChannels()
{
  /*digitalWrite(4, HIGH);
    digitalWrite(13, HIGH);
    delayMicroseconds(2000);
    digitalWrite(4, LOW);
    digitalWrite(13, LOW);
    delayMicroseconds(2000);
    delayMicroseconds(2000);
    delayMicroseconds(2000);
    delayMicroseconds(2000);
    delayMicroseconds(2000);
    delayMicroseconds(2000);
    delayMicroseconds(2000);
    delayMicroseconds(2000);
    delayMicroseconds(2000);*/
  digitalWrite(13, HIGH);
  digitalWrite(4, HIGH);
  delay(1);
  //digitalWrite(13, LOW);
  delay(17);
  digitalWrite(13, LOW);
  digitalWrite(4, LOW);
  delay(2);
}

void voltageMeasurement()
{
  int sensorValue = analogRead(A0);
  // Convert the analog reading (which goes from 0 - 1023) to a voltage (0 - 5V):
  float voltage = sensorValue * (5.0 / 1023.0);
  Serial.print("Voltage #");
  Serial.println(voltage);
  if ((Previous_Voltage > 0.5) && (voltage == 0))
  {
    start_timer = micros();
    Zero_Changes_Counter++;
    Serial.print("Zero Changes: ");
    Serial.println(Zero_Changes_Counter);
  }
  if (Previous_Zero_Changes_Counter < Zero_Changes_Counter)
  {
    pulse_width = (( (volatile long)micros() - start_timer) / 2) * 2;
    Previous_Zero_Changes_Counter = Zero_Changes_Counter;
    Serial.print("Beep Width #");
    Serial.print(Zero_Changes_Counter);
    Serial.print(" : ");
    Serial.println(pulse_width);
  }
  Previous_Voltage = voltage;
}