# Contoso Electronics — Frequently Asked Questions (FAQ)

## Ordering & Shipping

### How long does shipping take?
- **Standard shipping:** 3-5 business days (€5.99, free on orders over €75)
- **Express shipping:** 1-2 business days (€14.99)
- We ship to EU, UK, US, Canada, Australia, Japan, and Singapore.

### Can I track my order?
Yes. Once your order ships, you receive a tracking email with a link to the carrier's tracking page. You can also check order status at contoso-electronics.example.com/orders.

### Do you ship internationally?
We currently ship to: EU member states, United Kingdom, United States, Canada, Australia, Japan, and Singapore. We plan to expand to South Korea and Brazil in Q2 2026.

### Can I change my order after placing it?
Orders in **Pending** status can be modified or cancelled. Once an order moves to **Confirmed** (usually within 1 hour of payment), modifications are no longer possible — but you can still cancel. Orders that are **Shipped** cannot be cancelled; please use the return process instead.

---

## Returns & Refunds

### What is your return policy?
Products can be returned within **30 calendar days** of delivery. Items must be in original packaging and unused or like-new condition. See section 4 of the Employee & Policy Handbook for full terms.

### How do I initiate a return?
1. Log in at contoso-electronics.example.com/returns
2. Select the order and items to return
3. Print the prepaid return label (for defective items) or pay €5.99 return shipping
4. Drop off the package at any DHL pickup point
5. Refund is processed within 5-7 business days of receiving the item

### Are software licenses refundable?
No. Opened software or firmware license keys are non-refundable.

### What if my product arrives damaged?
Contact support immediately with photos of the damage and your order number. We will ship a replacement at no cost or issue a full refund — your choice.

---

## Products

### Is the SmartHub 400 compatible with my existing smart home devices?
The SmartHub 400 supports **Matter 1.3, Zigbee 3.0, Z-Wave, Thread, and Wi-Fi 6E**. It is compatible with Apple HomeKit, Google Home, Amazon Alexa, and Samsung SmartThings. Most major smart home devices from the last 3-4 years will work.

### Does the SmartHub 400 require an internet connection?
No. The SmartHub 400 is designed for **local-first operation**. All automations, voice control, and device management work without an internet connection. An internet connection is only needed for remote access, OTA firmware updates, and cloud-based voice assistants.

### How long does the TempSense Pro battery last?
The TempSense Pro uses a CR2477 coin cell battery that lasts approximately **18 months** under normal use (reporting every 5 minutes).

### Is the SecureView 360 camera waterproof?
Yes. The SecureView 360 is rated **IP67**, meaning it is dust-tight and can withstand immersion in water up to 1 meter for 30 minutes. It is designed for both indoor and outdoor use.

### When will the SmartLock BT2 be back in stock?
The SmartLock BT2 is expected to be restocked on **March 1, 2026**. You can sign up for restock notifications on the product page.

---

## Warranty & Support

### What warranty do Contoso products come with?
- **Consumer electronics:** 2-year manufacturer warranty
- **Industrial IoT sensors:** 3-year manufacturer warranty
- Warranty covers defects in materials and workmanship but excludes damage from misuse.

### What is ContosoGuard?
ContosoGuard is our extended warranty program. For a one-time fee, you can extend your coverage by 1-3 years and add accidental damage protection (drops, spills) with a €50 deductible per claim. ContosoGuard must be purchased within 30 days of the product purchase.

### How do I file a warranty claim?
Visit contoso-electronics.example.com/warranty. You will need your original invoice and the product's serial number. Claims are typically processed within 3-5 business days.

### How do I contact customer support?
- **Email:** support@contoso-electronics.example.com
- **Phone:** +31 20 555 0199
- **Hours:** Mon-Fri 08:00-20:00 CET, Sat 10:00-16:00 CET
- Average response time: email within 4 hours, phone wait under 2 minutes

---

## Technical

### What is the SmartHub API rate limit?
100 requests per second on the local network. There is no rate limiting for typical home automation use — this limit exists to prevent runaway scripts from overloading the hub.

### Can I use RTSP to stream from the SecureView 360?
Yes. The SecureView 360 supports RTSP streaming. The stream URL format is:
`rtsp://<camera-ip>:554/live`
You can use this with any RTSP-compatible NVR, Home Assistant, or VLC.

### Does the VibraSense X1 work in hazardous environments?
The VibraSense X1 is **ATEX Zone 2 certified**, meaning it can be used in areas where explosive atmospheres may occasionally occur. It is NOT certified for Zone 0 or Zone 1 environments.

### What communication protocols does the FlowMeter M3 support?
The FlowMeter M3 supports **RS485 Modbus RTU**, **Modbus TCP** (via optional Ethernet adapter), and **MQTT** (via built-in Wi-Fi or Ethernet). Data can be pushed to any MQTT broker.
