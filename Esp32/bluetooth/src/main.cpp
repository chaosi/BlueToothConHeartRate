#include <Arduino.h>
//Arduino 1.0+ Compatible only
//Arduino 1.0+ Compatible only
//Arduino 1.0+ Compatible only

// Code to retrieve heartrate information from the Polar Heart Rate Monitor Interface via I2C
// Part: http://www.sparkfun.com/products/8661
// Article:  http://bildr.org/2011/08/heartrate-arduino/
// Project Tutorial: http://amandasparling.com/?p=213

#include "Wire.h"

#define HRMI_I2C_ADDR      127
#define HRMI_HR_ALG        1   // 1= average sample, 0 = raw sample
int lowHeartRate = 50; //BPM - change as needed
int highHeartRate = 200; //BPM - change as needed
int vibemotor = 13;	// motor is connected to digital pin 13


#include "BluetoothSerial.h" //Header File for Serial Bluetooth, will be added by default into Arduino
BluetoothSerial ESP_BT; //Object for Bluetooth
int incoming;




void writeRegister(int deviceAddress, byte address, byte val) {
  //I2C command to send data to a specific address on the device
  Wire.beginTransmission(deviceAddress); // start transmission to device 
  Wire.write(address);       // send register address
  Wire.write(val);         // send value to write
  Wire.endTransmission();     // end transmission
}

boolean hrmiGetData(byte addr, byte numBytes, byte* dataArray){
  //Get data from heart rate monitor and fill dataArray byte with responce
  //Returns true if it was able to get it, false if not
  Wire.requestFrom(addr, numBytes);
  if (Wire.available()) {

    for (int i=0; i<numBytes; i++){
      dataArray[i] = Wire.read();
    }

    return true;
  }
  else{
    return false;
  }
  
}

void setupHeartMonitor(int type){
  //setup the heartrate monitor
  Wire.begin();
  writeRegister(HRMI_I2C_ADDR, 0x53, type); // Configure the HRMI with the requested algorithm mode
}


int getHeartRate(){
  //get and return heart rate
  //returns 0 if we couldnt get the heart rate
  byte i2cRspArray[3]; // I2C response array
  i2cRspArray[2] = 0;

  writeRegister(HRMI_I2C_ADDR,  0x47, 0x1); // Request a set of heart rate values 

  if (hrmiGetData(127, 3, i2cRspArray)) {
    return i2cRspArray[2];
  }
  else{
    return 0;
  }
  
}


void setup(){

  Serial.begin(9600); //Start Serial monitor in 9600
  ESP_BT.begin("ESP32_HeartRate"); //Name of your Bluetooth Signal
  Serial.println("Bluetooth Device is Ready to Pair");

  setupHeartMonitor(HRMI_HR_ALG); 
  pinMode(vibemotor, HIGH);	
}

void loop(){

  int heartRate = getHeartRate();
  //Serial.println(heartRate);
  ESP_BT.println(heartRate);

  delay(1000); //just here to slow down the checking to once a second
  
   int vibe = map(heartRate, lowHeartRate, highHeartRate, 0, 1024);
 
 if (heartRate > 125) {
   digitalWrite(vibemotor, HIGH);
 }
  else{
    digitalWrite(vibemotor, LOW);
  }
}