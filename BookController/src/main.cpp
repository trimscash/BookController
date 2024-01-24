#include <Arduino.h>
#include <BLE2902.h>
#include <BLEDevice.h>
#include <BLEServer.h>
#include <BLEUtils.h>

#include "esp_system.h"

#define LEFT_BUTTON 18
#define RIGHT_BUTTON 5

#define LEFT_LED 33
#define RIGHT_LED 32

#define RIGHT_SENSOR 16
#define LEFT_SENSOR 17

#define SERVICE_UUID "da3bb75d-0ea5-43f0-80d0-52fcda5567b5"
#define CHARACTERISTIC_UUID "118bbbd5-2554-4272-93a3-689dec6d7182"
#define DEVICE_NAME "BookController "

#define CHATTERING 40

BLECharacteristic *pCharacteristic;
bool deviceConnected = false;
enum SensorState {
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
	pinMode(LEFT_LED, OUTPUT);
	pinMode(RIGHT_LED, OUTPUT);
	digitalWrite(LEFT_LED, LOW);
	digitalWrite(RIGHT_LED, LOW);

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

SensorState leftChattering() {
	static unsigned long last = millis();
	static SensorState lastState = SensorState::low;
	if (millis() - last > CHATTERING) {
		SensorState nowState = digitalRead(LEFT_SENSOR) == HIGH ? SensorState::high : SensorState::low;
		if (nowState == lastState) {
			return nowState;
		}
		lastState = nowState;
		last = millis();
		return SensorState::low;
	}
	return SensorState::low;
}

SensorState rightChattering() {
	static unsigned long last = millis();
	static SensorState lastState = SensorState::low;
	if (millis() - last > CHATTERING) {
		SensorState nowState = digitalRead(RIGHT_SENSOR) == HIGH ? SensorState::high : SensorState::low;
		// Serial.println(nowState);
		if (nowState == lastState) {
			return nowState;
		}
		lastState = nowState;
		last = millis();
		return SensorState::low;
	}
	return SensorState::low;
}

void sendData(SendData d) {
	uint8_t send[1] = {d};
	Serial.println(send[0]);
	pCharacteristic->setValue(send, sizeof(uint8_t));
	pCharacteristic->notify();
	if (SendData::left == d) {
		digitalWrite(LEFT_LED, HIGH);
		digitalWrite(RIGHT_LED, LOW);
	} else {
		digitalWrite(LEFT_LED, LOW);
		digitalWrite(RIGHT_LED, HIGH);
	}
}

void loop() {
	static SensorState prevLeft = SensorState::low;
	static SensorState prevRight = SensorState::low;
	static bool leftFlag = false;
	static bool rightFlag = false;

	SensorState nowLeft = leftChattering();
	SensorState nowRight = rightChattering();

	if (prevLeft == SensorState::low && nowLeft == SensorState::high) {
		if (rightFlag) {
			sendData(SendData::right);
			rightFlag = false;
		} else {
			leftFlag = true;
		}
	}

	if (prevRight == SensorState::low && nowRight == SensorState::high) {
		if (leftFlag) {
			sendData(SendData::left);
			leftFlag = false;
		} else {
			rightFlag = true;
		}
	}

	prevLeft = nowLeft;
	prevRight = nowRight;
}
