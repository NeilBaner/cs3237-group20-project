#include <ESP8266WiFi.h>
#include <ESP8266HTTPClient.h>
#include <WiFiClient.h>
#include <MPU9250_WE.h>
#include <Wire.h>
#define MPU9250_ADDR 0x68

//Wifi Information 
const char* ssid = "Shreyas-Wifi"; // To fill out
const char* password = "8alpha0208912677298las"; // To fill out

WiFiClient wifiClient;
const char* laptopAt = "http://192.168.10.114:3237/"; //change to your Laptop's IP
MPU9250_WE myMPU9250 = MPU9250_WE(MPU9250_ADDR);

void setup(void){

  //Setting up Wifi Connection for WeMos
  Serial.begin(115200);
  WiFi.begin(ssid, password);
  Serial.println("");
  
  // Wait for connection
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("");
  Serial.print("Connected to ");
  Serial.println(ssid);
  Serial.print("IP address: ");
  Serial.println(WiFi.localIP());

  //Setting up MPU9250
  Wire.begin();
  if(!myMPU9250.init()){
    Serial.println("MPU9250 does not respond");
  }
  else{
    Serial.println("MPU9250 is connected");
  }
  if(!myMPU9250.initMagnetometer()){
    Serial.println("Magnetometer does not respond");
  }
  else{
    Serial.println("Magnetometer is connected");
  }

  //Calibration and Offsetting steps for MPU9250 
  Serial.println("Position you MPU9250 flat and don't move it - calibrating...");
  delay(1000);
  myMPU9250.autoOffsets();
  Serial.println("Done!");

  //Enabling and initializing the digital low pass band filter for Gyroscope
  myMPU9250.enableGyrDLPF();
  myMPU9250.setGyrDLPF(MPU9250_DLPF_6);
  myMPU9250.setSampleRateDivider(5);
  myMPU9250.setGyrRange(MPU9250_GYRO_RANGE_250);

  //Enabling and initializing the digital low pass band filter for Accelorometer
  myMPU9250.setAccRange(MPU9250_ACC_RANGE_2G);
  myMPU9250.enableAccDLPF(true);
  myMPU9250.setAccDLPF(MPU9250_DLPF_6);

  //Enabling Axes for accelorometer and gyroscope measurements
  myMPU9250.setMagOpMode(AK8963_CONT_MODE_100HZ);
  delay(200);
  
}

void loop() {
  // put your main code here, to run repeatedly

  Serial.print("Sending...");
  if (WiFi.status() == WL_CONNECTED){
    HTTPClient http;

    xyzFloat gValue = myMPU9250.getGValues();
    xyzFloat gyr = myMPU9250.getGyrValues();
    xyzFloat magValue = myMPU9250.getMagValues();
    float temp = myMPU9250.getTemperature();
    float resultantG = myMPU9250.getResultantG(gValue);
  
    //Printing Accelorometer data
    Serial.print(gValue.x);
    Serial.print(",");
    Serial.print(gValue.y);
    Serial.print(",");
    Serial.print(gValue.z);
    Serial.println("");
    String url = laptopAt;
    url += "data?gyro_x=";
    url += gyr.x;
    url +="&gyro_y="; 
    url += gyr.y;
    url +="&gyro_z="; 
    url += gyr.z;
    url +="&accel_x="; 
    url += gValue.x;
    url +="&accel_y="; 
    url += gValue.y;
    url +="&accel_z="; 
    url += gValue.z;
    url +="&resultantG="; 
    url += resultantG;    
    //hardcoded values for example only
    
    http.begin(wifiClient,url);
    int returnCode = http.GET();   //perform a HTTP GET request
    
    if (returnCode > 0){
      String payload = http.getString();
      Serial.println(payload);
    }
    http.end();
    
  } else {
    Serial.println("WiFi disconnected");
  }
  delay(100); //Having a sampling rate of 10Hz
}
