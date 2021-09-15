import board
import digitalio
import time
import storage, usb_cdc
import board, digitalio
import usb_hid
import analogio


# In this example, the button is wired to connect D2 to +V when pushed.
button = digitalio.DigitalInOut(board.GP2)
button.pull = digitalio.Pull.DOWN

# Disable devices only if button is not pressed.
if not button.value:
	storage.disable_usb_drive()
	usb_cdc.disable()

boot2 = digitalio.DigitalInOut(board.GP5)
boot2.direction = digitalio.Direction.OUTPUT
boot = digitalio.DigitalInOut(board.GP4)
boot.direction = digitalio.Direction.OUTPUT

def bootUp():
    time.sleep(1)
    boot2.value = True
    boot.value = True
    time.sleep(0.030)
    boot2.value = False
    boot.value = False

bootUp()