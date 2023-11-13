#include <Arduino.h>
#include <BLEDevice.h>
#include <BLEServer.h>
#include <BLEUtils.h>

#define LEFT_BUTTON 18
#define RIGHT_BUTTON 5

#define SERVICE_UUID "da3bb75d-0ea5-43f0-80d0-52fcda5567b5"
#define CHARACTERISTIC_UUID "118bbbd5-2554-4272-93a3-689dec6d7182"
#define DEVICE_NAME "ESP32 BookController"

#define CHATTERING 40

BLECharacteristic *pCharacteristic;

enum ButtonState {
	high,
	low
};

enum SendData : uint8_t {
	left = 0,
	right = 1
};

void setup() {
	Serial.begin(115200);
	pinMode(LEFT_BUTTON, INPUT_PULLDOWN);
	pinMode(RIGHT_BUTTON, INPUT_PULLDOWN);

	BLEDevice::init(DEVICE_NAME);
	BLEServer *pServer = BLEDevice::createServer();
	BLEService *pService = pServer->createService(SERVICE_UUID);
	pCharacteristic = pService->createCharacteristic(
		CHARACTERISTIC_UUID,
		BLECharacteristic::PROPERTY_NOTIFY);
	pService->start();

	BLEAdvertising *pAdvertising = BLEDevice::getAdvertising();
	pAdvertising->addServiceUUID(SERVICE_UUID);
	pAdvertising->setScanResponse(true);
	pAdvertising->setMinPreferred(0x06);
	pAdvertising->setMinPreferred(0x12);
	pAdvertising->start();
}

ButtonState leftChattering() {
	static unsigned long last = millis();
	static ButtonState lastState = ButtonState::low;
	if (millis() - last > CHATTERING) {
		ButtonState nowState = digitalRead(LEFT_BUTTON) == HIGH ? ButtonState::high : ButtonState::low;
		if (nowState == lastState) {
			return nowState;
		}
		lastState = nowState;
		last = millis();
		return ButtonState::low;
	}
	return ButtonState::low;
}

ButtonState rightChattering() {
	static unsigned long last = millis();
	static ButtonState lastState = ButtonState::low;
	if (millis() - last > CHATTERING) {
		ButtonState nowState = digitalRead(RIGHT_BUTTON) == HIGH ? ButtonState::high : ButtonState::low;
		if (nowState == lastState) {
			return nowState;
		}
		lastState = nowState;
		last = millis();
		return ButtonState::low;
	}
	return ButtonState::low;
}

void loop() {
	static ButtonState prevLeft = ButtonState::low;
	static ButtonState prevRight = ButtonState::low;
	ButtonState nowLeft = leftChattering();
	ButtonState nowRight = rightChattering();

	if (prevLeft == ButtonState::low && nowLeft == ButtonState::high) {
		SendData send = SendData::left;
		Serial.println(send);
		pCharacteristic->setValue((uint8_t *)&send, sizeof(int));
		pCharacteristic->notify();
	}

	if (prevRight == ButtonState::low && nowRight == ButtonState::high) {
		SendData send = SendData::right;
		Serial.println(send);
		pCharacteristic->setValue((uint8_t *)&send, sizeof(int));
		pCharacteristic->notify();
	}

	prevLeft = nowLeft;
	prevRight = nowRight;
}
