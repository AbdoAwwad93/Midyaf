# Midyaf API Documentation

## Table of Contents
- [Midyaf API Documentation](#midyaf-api-documentation)
  - [Table of Contents](#table-of-contents)
  - [Room Endpoints](#room-endpoints)
    - [1. Get All Rooms](#1-get-all-rooms)
    - [2. Get Room by ID](#2-get-room-by-id)
    - [3. Get Rooms by Property](#3-get-rooms-by-Property)
    - [4. Add Room](#4-add-room)
    - [5. Update Room](#5-update-room)
    - [6. Delete Room](#6-delete-room)
    - [7. Upload Room Image](#7-upload-room-image)
    - [8. Delete Room Image](#8-delete-room-image)
  - [Room Type Endpoints](#room-type-endpoints)
    - [1. Get All Room Types](#1-get-all-room-types)
    - [2. Get Room Type by ID](#2-get-room-type-by-id)
    - [3. Get Room Types by Property](#3-get-room-types-by-Property)
    - [4. Add Room Type](#4-add-room-type)
    - [5. Update Room Type](#5-update-room-type)
    - [6. Delete Room Type](#6-delete-room-type)
  - [Authentication](#authentication)
    - [How to Use JWT Token](#how-to-use-jwt-token)
  - [Response Format](#response-format)
    - [Response Fields](#response-fields)
  - [Account Endpoints](#account-endpoints)
    - [1. User Registration](#1-user-registration)
    - [2. Admin Registration](#2-admin-registration)
    - [3. User Login](#3-user-login)
    - [4. Forgot Password](#4-forgot-password)
    - [5. Reset Password](#5-reset-password)
  - [Property Endpoints](#Property-endpoints)
    - [1. Get All Propertys](#1-get-all-Propertys)
    - [2. Add Property](#2-add-Property)
    - [3. Update Property](#3-update-Property)
    - [4. Delete Property](#4-delete-Property)
    - [5. Search Propertys](#5-search-Propertys)
    - [6. Upload Property Image](#6-upload-Property-image)
    - [7. Delete Property Image](#7-delete-Property-image)
  - [Room Endpoints](#room-endpoints-1)
    - [1. Get All Rooms](#1-get-all-rooms-1)
    - [2. Get Room by ID](#2-get-room-by-id-1)
    - [3. Get Rooms by Property](#3-get-rooms-by-Property-1)
    - [4. Add Room](#4-add-room-1)
    - [5. Update Room](#5-update-room-1)
    - [6. Delete Room](#6-delete-room-1)
    - [7. Upload Room Image](#7-upload-room-image-1)
    - [8. Delete Room Image](#8-delete-room-image-1)
  - [Reservation Endpoints](#reservation-endpoints)
    - [1. Get All Reservations](#1-get-all-reservations)
    - [2. Get Reservation by ID](#2-get-reservation-by-id)
    - [3. Get My Reservations](#3-get-my-reservations)
    - [4. Create Reservation](#4-create-reservation)
    - [5. Update Reservation](#5-update-reservation)
    - [6. Update Reservation Status](#6-update-reservation-status)
    - [7. Cancel Reservation](#7-cancel-reservation)
  - [Review Endpoints](#review-endpoints)
    - [1. Get All Reviews](#1-get-all-reviews)
    - [2. Get Review by ID](#2-get-review-by-id)
    - [3. Get Reviews by Property](#3-get-reviews-by-Property)
    - [4. Get My Reviews](#4-get-my-reviews)
    - [5. Create Review](#5-create-review)
    - [6. Update Review](#6-update-review)
    - [7. Delete Review](#7-delete-review)
  - [HTTP Status Codes](#http-status-codes)
  - [Error Handling](#error-handling)
    - [Validation Errors](#validation-errors)
    - [Authentication Errors](#authentication-errors)
    - [Authorization Errors](#authorization-errors)

...

## Room Endpoints

### 1. Get All Rooms

Retrieve a list of all rooms.

**Endpoint:** `GET /api/Room`

**Authentication:** Not required

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "All rooms retrieved successfully",
  "data": [
    {
      "id": 1,
      "roomNumber": "101",
      "price": 150.00,
      "capacity": 2,
      "isAvailable": true,
      "imagesUrls": ["https://example.com/room1.jpg"],
      "PropertyId": 1,
      "roomTypeId": 5,
      "roomType": { "id": 5, "name": "Deluxe Suite" }
    }
  ],
  "errors": []
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
  "success": true,
  "message": "Room retrieved successfully",
  "data": {
    "id": 1,
    "roomNumber": "101",
    "price": 150.00,
    "capacity": 2,
    "isAvailable": true,
    "imagesUrls": [],
    "PropertyId": 1,
    "roomTypeId": 5
  },
  "errors": []
}
```

---

### 3. Get Rooms by Property

Retrieve all rooms for a specific Property.

**Endpoint:** `GET /api/Room/Property/{PropertyId}`

**Authentication:** Not required

**URL Parameters:**
- `PropertyId` (integer): Property ID

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Rooms for Property 1 retrieved successfully",
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
  "PropertyId": 1,
  "roomTypeId": 5
}
```

**Validation Rules:**
- `roomNumber`: Required
- `price`: Required, must be greater than 0
- `capacity`: Required, between 1 and 20
- `isAvailable`: Optional, defaults to true
- `imagesUrls`: Optional
- `PropertyId`: Required, must reference existing Property
- `roomTypeId`: Optional, must reference existing room type if provided

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Room added successfully",
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
  "success": true,
  "message": "Room deleted successfully",
  "data": null
}
```

---

### 7. Upload Room Image

Upload an image for a room. **Admin or Manager access required.**

**Endpoint:** `POST /api/Room/{id}/images`

**Authentication:** Required (Admin or Manager role)

**Request Type:** `multipart/form-data`

**Form Fields:**
- `file`: The image file (jpg, jpeg, png, webp)

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Image added successfully",
  "data": ["/uploads/rooms/guid.jpg"]
}
```

---

### 8. Delete Room Image

Delete an image from a room. **Admin or Manager access required.**

**Endpoint:** `DELETE /api/Room/{id}/images?imageUrl={imageUrl}`

**Authentication:** Required (Admin or Manager role)

**Query Parameters:**
- `imageUrl` (string): The relative URL of the image to delete

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Image removed successfully",
  "data": []
}
```

---

## Room Type Endpoints

### 1. Get All Room Types

Retrieve a list of all room types.

**Endpoint:** `GET /api/RoomType`

**Authentication:** Not required

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Room Types retrieved successfully",
  "data": [
    {
      "id": 1,
      "name": "Deluxe Suite",
      "description": "A luxury suite with sea view",
      "price": 250.00,
      "capacity": 2,
      "PropertyId": 1
    }
  ],
  "errors": []
}
```

---

### 2. Get Room Type by ID

Retrieve a specific room type by its ID.

**Endpoint:** `GET /api/RoomType/{id}`

**Authentication:** Not required

**URL Parameters:**
- `id` (integer): Room Type ID

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Room Type retrieved successfully",
  "data": {
      "id": 1,
      "name": "Deluxe Suite",
      "description": "A luxury suite with sea view",
      "price": 250.00,
      "capacity": 2,
      "PropertyId": 1
  },
  "errors": []
}
```

---

### 3. Get Room Types by Property

Retrieve all room types for a specific Property.

**Endpoint:** `GET /api/RoomType/Property/{PropertyId}`

**Authentication:** Not required

**URL Parameters:**
- `PropertyId` (integer): Property ID

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Room Types retrieved successfully",
  "data": [...]
}
```

---

### 4. Add Room Type

Create a new room type. **Admin or Manager access required.**

**Endpoint:** `POST /api/RoomType/add`

**Authentication:** Required (Admin or Manager role)

**Request Body:**
```json
{
  "name": "Deluxe Suite",
  "description": "A luxury suite with sea view",
  "price": 250.00,
  "capacity": 2,
  "PropertyId": 1
}
```

**Validation Rules:**
- `name`: Required
- `price`: Required, must be greater than 0
- `capacity`: Required
- `PropertyId`: Required

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Room Type created successfully",
  "data": {...}
}
```

---

### 5. Update Room Type

Update an existing room type. **Admin or Manager access required.**

**Endpoint:** `PATCH /api/RoomType/edit/{id}`

**Authentication:** Required (Admin or Manager role)

**URL Parameters:**
- `id` (integer): Room Type ID

**Request Body:** Same as Add Room Type

---

### 6. Delete Room Type

Delete a room type. **Admin or Manager access required.**

**Endpoint:** `DELETE /api/RoomType/delete/{id}`

**Authentication:** Required (Admin or Manager role)

**URL Parameters:**
- `id` (integer): Room Type ID

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Room Type deleted successfully",
  "data": null
}
```

---

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

**Success Response:**
```json
{
  "success": true,
  "message": "Success message",
  "data": {}
}
```

**Failure Response:**
```json
{
  "success": false,
  "message": "Error description",
  "data": null,
  "errors": ["Specific error 1", "Specific error 2"]
}
```

### Response Fields

- `success` (boolean): Indicates whether the request was successful
- `message` (string): Human-readable message describing the result
- `data` (object|null): Response data (null if no data to return)
- `errors` (array): Array of error messages (empty array if successful)

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
  "confirmPassword": "password123",
  "Role": "User" //User OR Manager
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
  "success": true,
  "message": "User with role User created successfully",
  "data": null
}
```

**Error Response (400 Bad Request) — Email already exists:**
```json
{
  "success": false,
  "message": "Email already exists",
  "data": null
}
```

**Error Response (400 Bad Request) — Invalid role:**
```json
{
  "success": false,
  "message": "Validation failed",
  "data": null,
  "errors": ["Invalid Role it must be User Or Manager"]
}
```

**Error Response (400 Bad Request) — Validation failed:**
```json
{
  "success": false,
  "message": "Validation failed",
  "data": null,
  "errors": [
    "The Email field is required.",
    "The Password must be at least 6 characters."
  ]
}
```

**Error Response (400 Bad Request) — Registration failed:**
```json
{
  "success": false,
  "message": "User creation failed",
  "data": null,
  "errors": [
    "Passwords must have at least one non alphanumeric character.",
    "Passwords must have at least one uppercase ('A'-'Z')."
  ]
}
```

---

### 2. Admin Registration

Register a new admin account. **Admin access required.**

**Endpoint:** `POST /Account/signup/admin`

**Authentication:** Required (Admin role)

**Headers:**
```
Authorization: Bearer <admin-jwt-token>
```

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
  "confirmPassword": "password123",
  "Role": "Admin"
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
  "success": true,
  "message": "User with role Admin created successfully",
  "data": null
}
```

**Error Response (400 Bad Request) — Invalid role:**
```json
{
  "success": false,
  "message": "Validation failed",
  "data": null,
  "errors": ["Invalid Role it must be Admin"]
}
```

**Error Response (400 Bad Request) — Email already exists:**
```json
{
  "success": false,
  "message": "Email already exists",
  "data": null
}
```

---

### 3. User Login

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
  "success": true,
  "message": "Authentication successful",
  "data": {
    "firstName": "Abdulrahman",
    "lastName": "Awwad",
    "userName": "Awwad211",
    "email": "Awwad@example.com",
    "phoneNumber": "01555248446",
    "country": "Egypt",
    "city": "Assiut",
    "address": "Dairout",
    "role": "User",
    "loginToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
  },
  "errors": []
}
```

**Note:** The `loginToken` field contains the JWT token as a string. Store this token securely for authenticated requests.

**Error Response (400 Bad Request) — Validation failed:**
```json
{
  "success": false,
  "message": "Validation failed",
  "data": null,
  "errors": ["The Email field is required."]
}
```

**Error Response (401 Unauthorized):**
```json
{
  "success": false,
  "message": "Invalid Email or Password",
  "data": null
}
```

---

### 4. Forgot Password

Initiate password reset process by sending an OTP to the user's email.

**Endpoint:** `POST /Account/forgot-password`

**Authentication:** Not required

**Request Body:**
```json
{
  "email": "john.doe@example.com"
}
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "If the email exists, an OTP has been sent",
  "data": null
}
```

**Error Response (400 Bad Request) — Validation failed:**
```json
{
  "success": false,
  "message": "Validation failed",
  "data": null,
  "errors": ["The Email field is required."]
}
```

---

### 5. Reset Password

Reset password using the OTP received via email.

**Endpoint:** `POST /Account/reset-password`

**Authentication:** Not required

**Request Body:**
```json
{
  "email": "john.doe@example.com",
  "otp": "123456",
  "newPassword": "newpassword123",
  "confirmNewPassword": "newpassword123"
}
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Password has been reset successfully",
  "data": null
}
```

**Error Response (400 Bad Request) — Invalid reset request:**
```json
{
  "success": false,
  "message": "Invalid reset request",
  "data": null
}
```

**Error Response (400 Bad Request) — Invalid or expired OTP:**
```json
{
  "success": false,
  "message": "Invalid or expired OTP",
  "data": null
}
```

**Error Response (400 Bad Request) — Password reset failed:**
```json
{
  "success": false,
  "message": "Password reset failed",
  "data": null,
  "errors": [
    "Passwords must have at least one non alphanumeric character.",
    "Passwords must have at least one uppercase ('A'-'Z')."
  ]
}
```

**Error Response (400 Bad Request) — Validation failed:**
```json
{
  "success": false,
  "message": "Validation failed",
  "data": null,
  "errors": ["The Email field is required."]
}
```

---

## Property Endpoints

### 1. Get All Propertys

Retrieve a list of all Propertys.

**Endpoint:** `GET /Property`

**Authentication:** Not required

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "All Propertys retrived successfully",
  "data": [
    {
      "id": 1,
      "name": "Grand Property",
      "address": "123 Property Street",
      "city": "New York",
      "country": "United States",
      "managerId": "user-id-here",
      "reviews": [],
      "rooms": []
    }
  ],
  "errors": []
}
```

**Error Response (400 Bad Request):**
```json
{
  "success": false,
  "message": "Error occured while retriving Propertys",
  "data": null
}
```

---

### 2. Add Property

Create a new Property. **Admin access required.**

**Endpoint:** `POST /Property/add`

**Authentication:** Required (Admin role)

**Headers:**
```
Authorization: Bearer <admin-jwt-token>
Content-Type: application/json
```

**Request Body:**
```json
{
  "name": "Grand Property",
  "address": "123 Property Street",
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
  "success": true,
  "message": "Property added successfully",
  "data": {
    "id": 1,
    "name": "Grand Property",
    "address": "123 Property Street",
    "city": "New York",
    "country": "United States",
    "managerId": null,
    "reviews": [],
    "rooms": []
  },
  "errors": []
}
```

**Error Response (400 Bad Request):**
```json
{
  "success": false,
  "message": "Invalid Data",
  "data": null
}
```

---

### 3. Update Property

Update an existing Property. **Admin or Manager access required.**

**Endpoint:** `PATCH /Property/edit/{id}`

**Authentication:** Required (Admin or Manager role)

**Headers:**
```
Authorization: Bearer <jwt-token>
Content-Type: application/json
```

**URL Parameters:**
- `id` (integer): Property ID

**Request Body:**
```json
{
  "name": "Updated Property Name",
  "address": "456 New Address",
  "city": "Los Angeles",
  "country": "United States"
}
```

**Validation Rules:**
Same as Add Property endpoint

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Property edited successfully",
  "data": {
    "id": 1,
    "name": "Updated Property Name",
    "address": "456 New Address",
    "city": "Los Angeles",
    "country": "United States",
    "managerId": null,
    "reviews": [],
    "rooms": []
  },
  "errors": []
}
```

**Error Response (400 Bad Request):**
```json
{
  "success": false,
  "message": "No Property existed with this data",
  "data": null
}
```

---

### 4. Delete Property

Delete a Property. **Admin or Manager access required.**

**Endpoint:** `DELETE /Property/delete/{id}`

**Note:** If the endpoint doesn't work, try `DELETE /api/Property/Property/delete/{id}` as there may be a route inconsistency in the code.

**Authentication:** Required (Admin or Manager role)

**Headers:**
```
Authorization: Bearer <jwt-token>
```

**URL Parameters:**
- `id` (integer): Property ID

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Property removed successfully",
  "data": null
}
```

**Error Response (400 Bad Request):**
```json
{
  "success": false,
  "message": "there is no Property with this data",
  "data": null
}
```

---

---

### 5. Search Propertys

Search and filter Propertys.

**Endpoint:** `GET /Property/search`

**Authentication:** Not required

**Query Parameters:**
- `name` (string): Search by Property name
- `city` (string): Search by city
- `country` (string): Search by country
- `checkIn` (date): YYYY-MM-DD
- `checkOut` (date): YYYY-MM-DD
- `numberOfGuests` (int): Number of guests
- `minPrice` (decimal): Minimum price per night
- `maxPrice` (decimal): Maximum price per night
- `minRating` (double): Minimum average rating
- `sortBy` (string): name, price, rating
- `sortOrder` (string): asc, desc
- `page` (int): Page number (default 1)
- `pageSize` (int): Items per page (default 10)

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Found 5 Propertys",
  "data": {
    "items": [...],
    "totalCount": 5,
    "page": 1,
    "pageSize": 10
  },
  "errors": []
}
```

---

### 6. Upload Property Image

Upload an image for a Property. **Admin or Manager access required.**

**Endpoint:** `POST /Property/{id}/images`

**Authentication:** Required (Admin or Manager role)

**Request Type:** `multipart/form-data`

**Form Fields:**
- `file`: The image file (jpg, jpeg, png, webp)

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Image added successfully",
  "data": ["/uploads/Propertys/guid.jpg"]
}
```

---

### 7. Delete Property Image

Delete an image from a Property. **Admin or Manager access required.**

**Endpoint:** `DELETE /Property/{id}/images?imageUrl={imageUrl}`

**Authentication:** Required (Admin or Manager role)

**Query Parameters:**
- `imageUrl` (string): The relative URL of the image to delete

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Image removed successfully",
  "data": []
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
  "success": true,
  "message": "All rooms retrieved successfully",
  "data": [
    {
      "id": 1,
      "roomNumber": "101",
      "price": 150.00,
      "capacity": 2,
      "isAvailable": true,
      "imagesUrls": ["https://example.com/room1.jpg"],
      "PropertyId": 1
    }
  ],
  "errors": []
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
  "success": true,
  "message": "Room retrieved successfully",
  "data": {
    "id": 1,
    "roomNumber": "101",
    "price": 150.00,
    "capacity": 2,
    "isAvailable": true,
    "imagesUrls": [],
    "PropertyId": 1
  },
  "errors": []
}
```

---

### 3. Get Rooms by Property

Retrieve all rooms for a specific Property.

**Endpoint:** `GET /api/Room/Property/{PropertyId}`

**Authentication:** Not required

**URL Parameters:**
- `PropertyId` (integer): Property ID

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Rooms for Property 1 retrieved successfully",
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
  "PropertyId": 1
}
```

**Validation Rules:**
- `roomNumber`: Required
- `price`: Required, must be greater than 0
- `capacity`: Required, between 1 and 20
- `isAvailable`: Optional, defaults to true
- `imagesUrls`: Optional
- `PropertyId`: Required, must reference existing Property

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Room added successfully",
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
  "success": true,
  "message": "Room deleted successfully",
  "data": null
}
```

---

---

### 7. Upload Room Image

Upload an image for a room. **Admin or Manager access required.**

**Endpoint:** `POST /api/Room/{id}/images`

**Authentication:** Required (Admin or Manager role)

**Request Type:** `multipart/form-data`

**Form Fields:**
- `file`: The image file (jpg, jpeg, png, webp)

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Image added successfully",
  "data": ["/uploads/rooms/guid.jpg"]
}
```

---

### 8. Delete Room Image

Delete an image from a room. **Admin or Manager access required.**

**Endpoint:** `DELETE /api/Room/{id}/images?imageUrl={imageUrl}`

**Authentication:** Required (Admin or Manager role)

**Query Parameters:**
- `imageUrl` (string): The relative URL of the image to delete

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Image removed successfully",
  "data": []
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
  "success": true,
  "message": "All reservations retrieved successfully",
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
  ],
  "errors": []
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
  "success": true,
  "message": "User reservations retrieved successfully",
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
  "success": true,
  "message": "Reservation created successfully",
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
  "success": true,
  "message": "Reservation status updated to Confirmed",
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
  "success": true,
  "message": "Reservation cancelled successfully",
  "data": null
}
```

---

## Review Endpoints

### 1. Get All Reviews

Retrieve all reviews.

**Endpoint:** `GET /api/Review`

**Authentication:** Not required

---

### 2. Get Review by ID

**Endpoint:** `GET /api/Review/{id}`

**Authentication:** Not required

---

### 3. Get Reviews by Property

**Endpoint:** `GET /api/Review/Property/{PropertyId}`

**Authentication:** Not required

---

### 4. Get My Reviews

**Endpoint:** `GET /api/Review/my`

**Authentication:** Required

---

### 5. Create Review

**Endpoint:** `POST /api/Review/add`

**Authentication:** Required

**Request Body:**
```json
{
  "comment": "Great Property, excellent service!",
  "rate": 5,
  "PropertyId": 1
}
```

**Validation Rules:**
- `comment`: Optional
- `rate`: Required, between 1 and 5
- `PropertyId`: Required, must reference existing Property

---

### 6. Update Review

Update your own review.

**Endpoint:** `PATCH /api/Review/edit/{id}`

**Authentication:** Required (owner only)

---

### 7. Delete Review

Delete your own review.

**Endpoint:** `DELETE /api/Review/delete/{id}`

**Authentication:** Required (owner only)

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

When validation fails, the API returns a `400 Bad Request` status with individual validation error messages in the `errors` array:

```json
{
  "success": false,
  "message": "Validation failed",
  "data": null,
  "errors": [
    "The Email field is required.",
    "The Password must be at least 6 characters."
  ]
}
```

### Authentication Errors

If an authenticated endpoint is accessed without a valid token or with an expired token, you will receive a `401 Unauthorized` response:

```json
{
  "success": false,
  "message": "User not authenticated",
  "data": null
}
```

### Authorization Errors

If you attempt to access an endpoint without the required role, you will receive a `403 Forbidden` response.
