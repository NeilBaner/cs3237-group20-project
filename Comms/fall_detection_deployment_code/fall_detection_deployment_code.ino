#include <ESP8266WiFi.h>
#include <ESP8266HTTPClient.h>
#include <WiFiClient.h>
#include <MPU9250_WE.h>
#include <Wire.h>

#define MPU9250_ADDR 0x68
#define BUZZER D3
#define LED D4
#define BUTTON D5

//Wifi Information 
const char* ssid = ""; // To fill out
const char* password = ""; // To fill out

const char* serverAt = "http://192.168.24.118:3237/"; //change to your Laptop's IP
MPU9250_WE myMPU9250 = MPU9250_WE(MPU9250_ADDR);

WiFiClient wifiClient;
volatile byte state = LOW;
volatile int last_interrupt_time = 0;

IRAM_ATTR void toggle() {
  unsigned long interrupt_time = millis(); // milis() Returns the number of milliseconds passed since the Arduino board began running the current program. 
  if (interrupt_time - last_interrupt_time > 200){
    state = ! state;
    last_interrupt_time = interrupt_time;
  }
}

void setup(){
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

  //Setting up Actuators
  pinMode(BUZZER, OUTPUT); // Set buzzer - pin 9 as an output
  pinMode(LED, OUTPUT);
  pinMode(BUTTON, INPUT_PULLUP);
  attachInterrupt(
   digitalPinToInterrupt(BUTTON),
   toggle,
   CHANGE 
  );
}

void loop(){

  if (WiFi.status() == WL_CONNECTED){
    HTTPClient http;
    xyzFloat gValue = myMPU9250.getGValues();
    xyzFloat gyr = myMPU9250.getGyrValues();
    xyzFloat magValue = myMPU9250.getMagValues();
    float temp = myMPU9250.getTemperature();
    float resultantG = myMPU9250.getResultantG(gValue);
    String url = serverAt;
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
  
    http.begin(wifiClient,url);

    //TODO: perform get request only if value if the resulatant gvalue is above 1.3
    int returnCode = http.GET();   //perform a HTTP GET request
    
    if (returnCode > 0){
      String payload = http.getString();
      Serial.println(payload);
    }
    /**
     TODO: Code over here is used to update the sat
    **/
    http.end();
    if(state){
      tone(BUZZER, 1000); // Send 1KHz sound signal...
      digitalWrite(LED, HIGH);
      delay(100);        // ...for 1 sec
      noTone(BUZZER);     // Stop sound...
      digitalWrite(LED, LOW);
      delay(100);        // ...for 1sec
    } 
  }
}
