## Authentication

The API uses **JWT (JSON Web Tokens)** for authentication. After successful login, you will receive a JWT token that must be included in subsequent authenticated requests.

### How to Use JWT Token

Include the token in the `Authorization` header of your requests:

```
Authorization: Bearer <your-jwt-token>
```

**Token Expiration:** JWT tokens expire after 30 minutes.

---

## Response Format

All API endpoints return responses in the following format:

```json
{
  "message": "Success message or error description",
  "isSuccess": true,
  "errors": null,
  "data": {}
}
```

### Response Fields

- `message` (string): Human-readable message describing the result
- `isSuccess` (boolean): Indicates whether the request was successful
- `errors` (array|null): Array of error messages (null if successful)
- `data` (object|null): Response data (null if no data to return)

---

## Account Endpoints

### 1. User Registration

Register a new user account.

**Endpoint:** `POST /Account/signup`

**Authentication:** Not required

**Request Body:**
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "userName": "johndoe",
  "email": "john.doe@example.com",
  "phoneNumber": "+1234567890",
  "country": "United States",
  "city": "New York",
  "address": "123 Main Street",
  "password": "password123",
  "confirmPassword": "password123"
}
```

**Validation Rules:**
- `firstName`: Required, letters only (a-z, A-Z)
- `lastName`: Required, letters only (a-z, A-Z)
- `userName`: Required
- `email`: Required, valid email address
- `phoneNumber`: Required, valid phone number format
- `country`: Required, letters only (a-z, A-Z)
- `city`: Required, letters only (a-z, A-Z)
- `address`: Required, letters only (a-z, A-Z)
- `password`: Required, minimum 6 characters
- `confirmPassword`: Required, must match `password`

**Success Response (201 Created):**
```json
{
  "message": "User with role User created successfully",
  "isSuccess": true,
  "errors": null,
  "data": {
    "email": "john.doe@example.com",
    "role": "User"
  }
}
```

**Error Response (400 Bad Request):**
```json
{
  "message": "Email already exists",
  "isSuccess": false,
  "errors": null,
  "data": null
}
```

---

### 2. Manager Registration

Register a new manager account. **Admin access required.**

**Endpoint:** `POST /Account/signup/manager`

**Authentication:** Required (Admin role)

**Headers:**
```
Authorization: Bearer <admin-jwt-token>
```

**Request Body:**
Same as User Registration (see above)

**Success Response (201 Created):**
```json
{
  "message": "User with role Manager created successfully",
  "isSuccess": true,
  "errors": null,
  "data": {
    "email": "manager@example.com",
    "role": "Manager"
  }
}
```

---

### 3. Admin Registration

Register a new admin account. **Admin access required.**

**Endpoint:** `POST /Account/signup/admin`

**Authentication:** Required (Admin role)

**Headers:**
```
Authorization: Bearer <admin-jwt-token>
```

**Request Body:**
Same as User Registration (see above)

**Success Response (201 Created):**
```json
{
  "message": "User with role Admin created successfully",
  "isSuccess": true,
  "errors": null,
  "data": {
    "email": "admin@example.com",
    "role": "Admin"
  }
}
```

---

### 4. User Login

Authenticate a user and receive a JWT token.

**Endpoint:** `POST /Account/Login`

**Authentication:** Not required

**Request Body:**
```json
{
  "email": "john.doe@example.com",
  "password": "password123",
  "rememberMe": false
}
```

**Validation Rules:**
- `email`: Required, valid email address
- `password`: Required
- `rememberMe`: Optional boolean

**Success Response (200 OK):**
```json
{
  "message": "Authentication successfu",
  "isSuccess": true,
  "errors": null,
  "data": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**Note:** The `data` field contains the JWT token as a string. Store this token securely for authenticated requests.

**Error Response (401 Unauthorized):**
```json
{
  "message": "Invalid Email or Password",
  "isSuccess": false,
  "errors": null,
  "data": null
}
```

---

## Hotel Endpoints

### 1. Get All Hotels

Retrieve a list of all hotels.

**Endpoint:** `GET /hotel`

**Authentication:** Not required

**Success Response (200 OK):**
```json
{
  "message": "All hotels retrived successfully",
  "isSuccess": true,
  "errors": null,
  "data": [
    {
      "id": 1,
      "name": "Grand Hotel",
      "address": "123 Hotel Street",
      "city": "New York",
      "country": "United States",
      "managerId": "user-id-here",
      "reviews": [],
      "rooms": []
    }
  ]
}
```

**Error Response (400 Bad Request):**
```json
{
  "message": "Error occured while retriving hotels",
  "isSuccess": false,
  "errors": null,
  "data": null
}
```

---

### 2. Add Hotel

Create a new hotel. **Admin access required.**

**Endpoint:** `POST /hotel/add`

**Authentication:** Required (Admin role)

**Headers:**
```
Authorization: Bearer <admin-jwt-token>
Content-Type: application/json
```

**Request Body:**
```json
{
  "name": "Grand Hotel",
  "address": "123 Hotel Street",
  "city": "New York",
  "country": "United States"
}
```

**Validation Rules:**
- `name`: Required, maximum 50 characters
- `address`: Required, alphanumeric and spaces only (a-z, A-Z, 0-9, spaces)
- `city`: Required, letters only (a-z, A-Z)
- `country`: Optional, letters only (a-z, A-Z)

**Success Response (200 OK):**
```json
{
  "message": "Hotel added successfully",
  "isSuccess": true,
  "errors": null,
  "data": {
    "id": 1,
    "name": "Grand Hotel",
    "address": "123 Hotel Street",
    "city": "New York",
    "country": "United States",
    "managerId": null,
    "reviews": [],
    "rooms": []
  }
}
```

**Error Response (400 Bad Request):**
```json
{
  "message": "Invalid Data",
  "isSuccess": false,
  "errors": null,
  "data": null
}
```

---

### 3. Update Hotel

Update an existing hotel. **Admin or Manager access required.**

**Endpoint:** `PATCH /hotel/edit/{id}`

**Authentication:** Required (Admin or Manager role)

**Headers:**
```
Authorization: Bearer <jwt-token>
Content-Type: application/json
```

**URL Parameters:**
- `id` (integer): Hotel ID

**Request Body:**
```json
{
  "name": "Updated Hotel Name",
  "address": "456 New Address",
  "city": "Los Angeles",
  "country": "United States"
}
```

**Validation Rules:**
Same as Add Hotel endpoint

**Success Response (200 OK):**
```json
{
  "message": "Hotel edited successfully",
  "isSuccess": true,
  "errors": null,
  "data": {
    "id": 1,
    "name": "Updated Hotel Name",
    "address": "456 New Address",
    "city": "Los Angeles",
    "country": "United States",
    "managerId": null,
    "reviews": [],
    "rooms": []
  }
}
```

**Error Response (400 Bad Request):**
```json
{
  "message": "No hotel existed with this data",
  "isSuccess": false,
  "errors": null,
  "data": null
}
```

---

### 4. Delete Hotel

Delete a hotel. **Admin or Manager access required.**

**Endpoint:** `DELETE /hotel/delete/{id}`

**Note:** If the endpoint doesn't work, try `DELETE /api/Hotel/hotel/delete/{id}` as there may be a route inconsistency in the code.

**Authentication:** Required (Admin or Manager role)

**Headers:**
```
Authorization: Bearer <jwt-token>
```

**URL Parameters:**
- `id` (integer): Hotel ID

**Success Response (200 OK):**
```json
{
  "message": "Hotel removed successfully",
  "isSuccess": true,
  "errors": null,
  "data": null
}
```

**Error Response (400 Bad Request):**
```json
{
  "message": "there is no hotel with this data",
  "isSuccess": false,
  "errors": null,
  "data": null
}
```

---

## Room Endpoints

### 1. Get All Rooms

Retrieve a list of all rooms.

**Endpoint:** `GET /api/Room`

**Authentication:** Not required

**Success Response (200 OK):**
```json
{
  "message": "All rooms retrieved successfully",
  "isSuccess": true,
  "errors": null,
  "data": [
    {
      "id": 1,
      "roomNumber": "101",
      "price": 150.00,
      "capacity": 2,
      "isAvailable": true,
      "imagesUrls": ["https://example.com/room1.jpg"],
      "hotelId": 1
    }
  ]
}
```

---

### 2. Get Room by ID

Retrieve a specific room by its ID.

**Endpoint:** `GET /api/Room/{id}`

**Authentication:** Not required

**URL Parameters:**
- `id` (integer): Room ID

**Success Response (200 OK):**
```json
{
  "message": "Room retrieved successfully",
  "isSuccess": true,
  "errors": null,
  "data": {
    "id": 1,
    "roomNumber": "101",
    "price": 150.00,
    "capacity": 2,
    "isAvailable": true,
    "imagesUrls": [],
    "hotelId": 1
  }
}
```

---

### 3. Get Rooms by Hotel

Retrieve all rooms for a specific hotel.

**Endpoint:** `GET /api/Room/hotel/{hotelId}`

**Authentication:** Not required

**URL Parameters:**
- `hotelId` (integer): Hotel ID

**Success Response (200 OK):**
```json
{
  "message": "Rooms for hotel 1 retrieved successfully",
  "isSuccess": true,
  "errors": null,
  "data": [...]
}
```

---

### 4. Add Room

Create a new room. **Admin or Manager access required.**

**Endpoint:** `POST /api/Room/add`

**Authentication:** Required (Admin or Manager role)

**Request Body:**
```json
{
  "roomNumber": "101",
  "price": 150.00,
  "capacity": 2,
  "isAvailable": true,
  "imagesUrls": ["https://example.com/room1.jpg"],
  "hotelId": 1
}
```

**Validation Rules:**
- `roomNumber`: Required
- `price`: Required, must be greater than 0
- `capacity`: Required, between 1 and 20
- `isAvailable`: Optional, defaults to true
- `imagesUrls`: Optional
- `hotelId`: Required, must reference existing hotel

**Success Response (200 OK):**
```json
{
  "message": "Room added successfully",
  "isSuccess": true,
  "errors": null,
  "data": {...}
}
```

---

### 5. Update Room

Update an existing room. **Admin or Manager access required.**

**Endpoint:** `PATCH /api/Room/edit/{id}`

**Authentication:** Required (Admin or Manager role)

**URL Parameters:**
- `id` (integer): Room ID

**Request Body:** Same as Add Room

---

### 6. Delete Room

Delete a room. **Admin or Manager access required.**

**Endpoint:** `DELETE /api/Room/delete/{id}`

**Authentication:** Required (Admin or Manager role)

**URL Parameters:**
- `id` (integer): Room ID

**Success Response (200 OK):**
```json
{
  "message": "Room deleted successfully",
  "isSuccess": true,
  "errors": null,
  "data": null
}
```

---

## Reservation Endpoints

### 1. Get All Reservations

Retrieve all reservations. **Admin or Manager access required.**

**Endpoint:** `GET /api/Reservation`

**Authentication:** Required (Admin or Manager role)

**Success Response (200 OK):**
```json
{
  "message": "All reservations retrieved successfully",
  "isSuccess": true,
  "errors": null,
  "data": [
    {
      "id": 1,
      "checkIn": "2026-02-01T00:00:00",
      "checkOut": "2026-02-05T00:00:00",
      "numberOfGuests": 2,
      "status": 0,
      "userId": "user-id",
      "rooms": [...]
    }
  ]
}
```

**Status Values:**
- `0`: Pending
- `1`: Confirmed
- `2`: Declined
- `3`: CheckIn
- `4`: CheckOut

---

### 2. Get Reservation by ID

Retrieve a specific reservation.

**Endpoint:** `GET /api/Reservation/{id}`

**Authentication:** Required

**URL Parameters:**
- `id` (integer): Reservation ID

---

### 3. Get My Reservations

Retrieve current user's reservations.

**Endpoint:** `GET /api/Reservation/my`

**Authentication:** Required

**Success Response (200 OK):**
```json
{
  "message": "User reservations retrieved successfully",
  "isSuccess": true,
  "errors": null,
  "data": [...]
}
```

---

### 4. Create Reservation

Create a new reservation.

**Endpoint:** `POST /api/Reservation/create`

**Authentication:** Required

**Request Body:**
```json
{
  "checkIn": "2026-02-01T00:00:00",
  "checkOut": "2026-02-05T00:00:00",
  "numberOfGuests": 2,
  "roomIds": [1, 2]
}
```

**Validation Rules:**
- `checkIn`: Required, cannot be in the past
- `checkOut`: Required, must be after checkIn
- `numberOfGuests`: Required, between 1 and 50
- `roomIds`: Required, array of room IDs to reserve

**Success Response (200 OK):**
```json
{
  "message": "Reservation created successfully",
  "isSuccess": true,
  "errors": null,
  "data": {...}
}
```

**Error Responses:**
- `400`: "Check-in date must be before check-out date"
- `400`: "Check-in date cannot be in the past"
- `400`: "Room with ID {id} not found"
- `400`: "Room {number} is not available"

---

### 5. Update Reservation

Update an existing reservation. **Admin or Manager access required.**

**Endpoint:** `PATCH /api/Reservation/edit/{id}`

**Authentication:** Required (Admin or Manager role)

**URL Parameters:**
- `id` (integer): Reservation ID

**Request Body:**
```json
{
  "checkIn": "2026-02-01T00:00:00",
  "checkOut": "2026-02-05T00:00:00",
  "numberOfGuests": 3,
  "status": 1,
  "roomIds": [1, 2]
}
```

---

### 6. Update Reservation Status

Change reservation status. **Admin or Manager access required.**

**Endpoint:** `PATCH /api/Reservation/status/{id}?status={status}`

**Authentication:** Required (Admin or Manager role)

**URL Parameters:**
- `id` (integer): Reservation ID

**Query Parameters:**
- `status` (integer): New status value (0-4)

**Success Response (200 OK):**
```json
{
  "message": "Reservation status updated to Confirmed",
  "isSuccess": true,
  "errors": null,
  "data": {...}
}
```

---

### 7. Cancel Reservation

Cancel a reservation (sets status to Declined).

**Endpoint:** `DELETE /api/Reservation/cancel/{id}`

**Authentication:** Required

**URL Parameters:**
- `id` (integer): Reservation ID

**Success Response (200 OK):**
```json
{
  "message": "Reservation cancelled successfully",
  "isSuccess": true,
  "errors": null,
  "data": null
}
```

---

## HTTP Status Codes

The API uses standard HTTP status codes:

- `200 OK`: Request succeeded
- `201 Created`: Resource created successfully
- `400 Bad Request`: Invalid request data or validation error
- `401 Unauthorized`: Authentication failed or token missing/invalid
- `403 Forbidden`: Insufficient permissions (wrong role)
- `500 Internal Server Error`: Server error

---

## Error Handling

### Validation Errors

When validation fails, the API returns a `400 Bad Request` status with model state errors in the response body. The exact format may vary, but typically includes field-specific error messages.

### Authentication Errors

If an authenticated endpoint is accessed without a valid token or with an expired token, you will receive a `401 Unauthorized` response.

### Authorization Errors

If you attempt to access an endpoint without the required role, you will receive a `403 Forbidden` response.