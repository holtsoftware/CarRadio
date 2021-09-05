import board
import digitalio
import time

run = digitalio.DigitalInOut(board.GP5)
run.direction = digitalio.Direction.OUTPUT
boot = digitalio.DigitalInOut(board.GP4)
boot.direction = digitalio.Direction.OUTPUT

def bootUp():
    print("preparting to boot waiting 1 second")
    time.sleep(1)
    run.value = True
    boot.value = True
    time.sleep(0.010)
    boot.value = False

bootUp()