#include <Arduino.h>
#include <BLE2902.h>
#include <BLEDevice.h>
#include <BLEServer.h>
#include <BLEUtils.h>

#include "esp_system.h"

#define LEFT_BUTTON 18
#define RIGHT_BUTTON 5

#define SERVICE_UUID "da3bb75d-0ea5-43f0-80d0-52fcda5567b5"
#define CHARACTERISTIC_UUID "118bbbd5-2554-4272-93a3-689dec6d7182"
#define DEVICE_NAME "BookController "

#define CHATTERING 40

BLECharacteristic *pCharacteristic;
bool deviceConnected = false;
enum ButtonState {
	high,
	low
};

enum SendData : uint8_t {
	left = 0,
	right = 1
};

class BLECallbacks : public BLEServerCallbacks {
	void onConnect(BLEServer *MyServer) {
		deviceConnected = true;
	}
	void onDisconnect(BLEServer *MyServer) {
		deviceConnected = false;
		MyServer->getAdvertising()->start();
	}
};

void setup() {
	uint8_t baseMac[6];
	esp_read_mac(baseMac, ESP_MAC_WIFI_STA);
	char baseMacChr[50] = {0};
	sprintf(baseMacChr, "%02X:%02X:%02X", baseMac[0], baseMac[1], baseMac[2]);

	Serial.begin(115200);
	pinMode(LEFT_BUTTON, INPUT_PULLDOWN);
	pinMode(RIGHT_BUTTON, INPUT_PULLDOWN);

	BLEDevice::init(DEVICE_NAME + std::string(baseMacChr));
	BLEServer *pServer = BLEDevice::createServer();
	pServer->setCallbacks(new BLECallbacks());
	BLEService *pService = pServer->createService(SERVICE_UUID);
	pCharacteristic = pService->createCharacteristic(
		CHARACTERISTIC_UUID, BLECharacteristic::PROPERTY_NOTIFY);
	BLE2902 *desc = new BLE2902();
	desc->setNotifications(true);

	pCharacteristic->addDescriptor(desc);

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
		uint8_t send[1] = {SendData::left};
		Serial.println(send[0]);
		pCharacteristic->setValue(send, sizeof(uint8_t));
		pCharacteristic->notify();
	}

	if (prevRight == ButtonState::low && nowRight == ButtonState::high) {
		uint8_t send[1] = {SendData::right};
		Serial.println(send[0]);
		pCharacteristic->setValue(send, sizeof(uint8_t));
		pCharacteristic->notify();
	}

	prevLeft = nowLeft;
	prevRight = nowRight;
}
