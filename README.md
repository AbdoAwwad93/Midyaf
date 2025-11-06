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