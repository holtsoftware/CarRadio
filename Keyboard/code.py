import board
import digitalio
import time
import usb_hid
import analogio
import digitalio


from adafruit_hid.keyboard import Keyboard
from adafruit_hid.keycode import Keycode

keyboard = Keyboard(usb_hid.devices)
one = analogio.AnalogIn(board.A0)
three = analogio.AnalogIn(board.A1)
led = digitalio.DigitalInOut(board.LED)
led.direction = digitalio.Direction.OUTPUT
led.value = True
time.sleep(0.5)
led.value = False

oneValues=[]
oneIndex=0

threeValues=[]
threeIndex=0

readings=30

for x in range(readings):
    oneValues.append(0)
    threeValues.append(0)

def tapKey(key):
    led.value = True
    keyboard.press(key)
    time.sleep(0.10)
    keyboard.release(key)
    led.value = False
    time.sleep(0.25)

def checkOne(num):
    print(num)
    if num > 39000 and num < 39999:
        print("volume down")
        tapKey(Keycode.F7)
    elif num > 55000 and num < 56999:
        print("next")
        tapKey(Keycode.N)


def checkThree(num):
    if num > 39000 and num < 39999:
        print("volume up")
        tapKey(Keycode.F8)
    elif num > 55000 and num < 56999:
        print("mode")
        tapKey(Keycode.P)
    elif num > 65000 and num < 66999:
        print("previous")
        tapKey(Keycode.V)

def readValues():
    global oneIndex
    global oneValues
    global threeValues
    global threeIndex
    global readings
    if oneIndex < readings:
        oneValues[oneIndex] = one.value
        oneIndex+=1
    else:
        average=0
        for x in oneValues:
            average += x
        average = average / readings
        oneIndex = 0
        checkOne(average)
    if threeIndex < readings:
        threeValues[threeIndex] = three.value
        threeIndex+=1
    else:
        average=0
        for x in threeValues:
            average += x
        average = average / readings
        threeIndex = 0
        checkThree(average)

time.sleep(1)
print("starting keyboard loop")
while True:
    readValues()
    #time.sleep(0.01)
    