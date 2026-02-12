# Contoso SmartHub 400 — API Reference

## Overview

The SmartHub 400 exposes a local REST API on port `8443` (HTTPS) for integration with custom automation systems. The API requires authentication via a bearer token generated from the SmartHub's local admin dashboard.

**Base URL:** `https://<smarthub-ip>:8443/api/v2`

---

## Authentication

All API requests require a bearer token in the `Authorization` header.

```http
Authorization: Bearer <your-token>
```

Tokens are generated in **Settings → Developer → API Keys** on the SmartHub's touchscreen or web dashboard. Tokens expire after 365 days and can be revoked at any time.

---

## Endpoints

### GET /devices

Returns all devices paired with the SmartHub.

**Response:**
```json
{
  "devices": [
    {
      "id": "zigbee-0x1234",
      "name": "Living Room Light",
      "type": "light",
      "protocol": "zigbee",
      "online": true,
      "last_seen": "2026-02-10T14:30:00Z",
      "properties": {
        "on": true,
        "brightness": 80,
        "color_temp": 3200
      }
    }
  ],
  "total": 42
}
```

### GET /devices/{id}

Returns a single device by ID.

**Parameters:**
- `id` (path, required): Device ID (e.g., `zigbee-0x1234`)

**Response:** Same structure as a single item from `/devices`.

**Error (404):**
```json
{
  "error": "device_not_found",
  "message": "No device with ID 'zigbee-0x9999'"
}
```

### POST /devices/{id}/command

Send a command to a device.

**Parameters:**
- `id` (path, required): Device ID

**Request body:**
```json
{
  "command": "set",
  "properties": {
    "on": true,
    "brightness": 50
  }
}
```

**Supported commands:**
| Command | Description | Device Types |
|---------|-------------|--------------|
| `set` | Set one or more properties | All |
| `toggle` | Toggle on/off state | Lights, switches, plugs |
| `identify` | Blink/beep for 10 seconds | All |
| `lock` | Lock the device | Locks |
| `unlock` | Unlock the device | Locks |

**Response (200):**
```json
{
  "status": "ok",
  "device_id": "zigbee-0x1234",
  "applied": {
    "on": true,
    "brightness": 50
  }
}
```

### GET /automations

Returns all configured automations (rules).

**Response:**
```json
{
  "automations": [
    {
      "id": "auto-001",
      "name": "Sunset Lights",
      "enabled": true,
      "trigger": {
        "type": "sun",
        "event": "sunset",
        "offset_minutes": -15
      },
      "conditions": [
        { "type": "device", "device_id": "zigbee-0x5678", "property": "presence", "value": true }
      ],
      "actions": [
        { "device_id": "zigbee-0x1234", "command": "set", "properties": { "on": true, "brightness": 60 } }
      ]
    }
  ]
}
```

### POST /automations

Create a new automation.

**Request body:** Same structure as a single automation (without `id`, which is auto-generated).

### DELETE /automations/{id}

Delete an automation by ID.

### GET /scenes

Returns saved scenes (pre-configured device states).

**Response:**
```json
{
  "scenes": [
    {
      "id": "scene-movie",
      "name": "Movie Night",
      "devices": [
        { "device_id": "zigbee-0x1234", "properties": { "on": true, "brightness": 15, "color_temp": 2700 } },
        { "device_id": "zigbee-0x5678", "properties": { "on": false } }
      ]
    }
  ]
}
```

### POST /scenes/{id}/activate

Activate a scene, applying all its device states simultaneously.

---

## Rate Limits

- **Local network:** 100 requests/second (no throttling for typical home use)
- **Concurrent connections:** Up to 20 simultaneous WebSocket + REST clients

## WebSocket API

Real-time device state changes are available via WebSocket at:

```
wss://<smarthub-ip>:8443/ws/events
```

Events are pushed as JSON messages:

```json
{
  "event": "device_state_changed",
  "device_id": "zigbee-0x1234",
  "timestamp": "2026-02-10T14:32:15Z",
  "changes": {
    "brightness": { "old": 80, "new": 50 }
  }
}
```

---

## Error Codes

| HTTP Code | Error | Description |
|-----------|-------|-------------|
| 400 | `invalid_request` | Malformed request body |
| 401 | `unauthorized` | Missing or invalid bearer token |
| 404 | `device_not_found` | Device ID does not exist |
| 409 | `device_offline` | Device is not reachable |
| 429 | `rate_limited` | Too many requests |
| 503 | `hub_busy` | Hub is updating firmware or restarting |
