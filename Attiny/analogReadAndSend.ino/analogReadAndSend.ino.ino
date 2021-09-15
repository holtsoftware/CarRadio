#include "TinyWireS.h"                  // wrapper class for I2C slave routines https://github.com/nadavmatalon/TinyWireS
#define I2C_SLAVE_ADDR  0x08 // i2c slave address (8)
#define ANALOGPIN1 A2
#define ANALOGPIN2 A3   

void setup() {
  // put your setup code here, to run once:
  TinyWireS.begin(I2C_SLAVE_ADDR);
  TinyWireS.onRequest(requestData);
}

void loop() {
}

void requestData()
{
  writeValue(analogRead(ANALOGPIN1), 1);
  writeValue(analogRead(ANALOGPIN2), 2);
}

void writeValue(int value, int pinId)
{
  int b = value & 0x3FF;
  b = b | ((pinId & 0xFF) << 10);
  TinyWireS.write((b >> 8) & 0xFF);
  TinyWireS.write(b & 0xFF);
}
