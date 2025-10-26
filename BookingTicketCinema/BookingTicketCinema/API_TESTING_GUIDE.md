# Cinema & Seating Management API Testing Guide

## Summary

All Cinema & Seating Management APIs have been successfully implemented and are ready for testing.

## What Was Implemented

### 1. Models Updated
- **Room**: Added `RoomType` enum (Type2D, Type3D, IMAX)
- **Seat**: Added `SeatStatus` enum (Empty, Booked, Broken)

### 2. DTOs Created
- `RoomDto.cs` - Create, Update, Response DTOs for Room
- `SeatGroupDto.cs` - Create, Update, Response DTOs for SeatGroup
- `SeatDto.cs` - Create, Update, Response DTOs for Seat
- `PriceRuleDto.cs` - Create, Update, Response DTOs for PriceRule

### 3. Repositories Created
- `RoomRepository` & `IRoomRepository`
- `SeatGroupRepository` & `ISeatGroupRepository`
- `SeatRepository` & `ISeatRepository`
- `PriceRuleRepository` & `IPriceRuleRepository`

### 4. Services Created
- `RoomService` & `IRoomService`
- `SeatGroupService` & `ISeatGroupService`
- `SeatService` & `ISeatService`
- `PriceRuleService` & `IPriceRuleService`

### 5. Endpoints Created
All endpoints are marked with `[AllowAnonymous]` for testing purposes.

#### Room Endpoints (`/api/rooms`)
- `GET /api/rooms` - Get all rooms
- `GET /api/rooms/{id}` - Get room by ID
- `POST /api/rooms` - Create new room
- `PUT /api/rooms/{id}` - Update room
- `DELETE /api/rooms/{id}` - Delete room

#### SeatGroup Endpoints (`/api/seatgroups`)
- `GET /api/seatgroups` - Get all seat groups
- `GET /api/seatgroups/{id}` - Get seat group by ID
- `GET /api/seatgroups/room/{roomId}` - Get seat groups by room
- `POST /api/seatgroups` - Create new seat group
- `PUT /api/seatgroups/{id}` - Update seat group
- `DELETE /api/seatgroups/{id}` - Delete seat group

#### Seat Endpoints (`/api/seats`)
- `GET /api/seats` - Get all seats
- `GET /api/seats/{id}` - Get seat by ID
- `GET /api/seats/room/{roomId}` - Get seats by room
- `POST /api/seats` - Create new seat
- `PUT /api/seats/{id}` - Update seat
- `DELETE /api/seats/{id}` - Delete seat

#### PriceRule Endpoints (`/api/pricerules`)
- `GET /api/pricerules` - Get all price rules
- `GET /api/pricerules/{id}` - Get price rule by ID
- `GET /api/pricerules/seatgroup/{seatGroupId}` - Get price rules by seat group
- `POST /api/pricerules` - Create new price rule
- `PUT /api/pricerules/{id}` - Update price rule
- `DELETE /api/pricerules/{id}` - Delete price rule

## How to Test

### Prerequisites
1. Make sure SQL Server is running and accessible
2. Run the database migration: `dotnet ef database update`
3. Restart the application to load the new endpoints

### Testing Methods

#### Option 1: Using Swagger UI
1. Navigate to `http://localhost:5098/swagger`
2. Find the Cinema & Seating Management endpoints
3. Click "Try it out" on any endpoint
4. Fill in the required parameters
5. Click "Execute"

#### Option 2: Using API_Tests.http file
1. Open the `API_Tests.http` file in Visual Studio or VS Code
2. Make sure the `@baseUrl` variable is set correctly
3. Click the "Run" button above each request

#### Option 3: Using curl or Postman
Use the examples from `API_Tests.http` file for request bodies.

## Example Test Flow

### 1. Create a Room
```json
POST /api/rooms
{
  "name": "Room 1 - 2D",
  "capacity": 100,
  "type": 0
}
```

### 2. Create Seat Groups for the Room
```json
POST /api/seatgroups
{
  "groupName": "Standard Seats",
  "type": 0,
  "roomId": 1
}
```

### 3. Create Seats
```json
POST /api/seats
{
  "seatNumber": "A1",
  "status": 0,
  "roomId": 1,
  "seatGroupId": 1
}
```

### 4. Create Price Rules
```json
POST /api/pricerules
{
  "basePrice": 50000,
  "dayOfWeek": 1,
  "slot": 0,
  "seatGroupId": 1
}
```

## Commits Made

1. `Add RoomType field to Room model`
2. `Add SeatStatus field to Seat model`
3. `Create DTOs for Room, SeatGroup, Seat, and PriceRule`
4. `Create repositories for Room, SeatGroup, Seat, and PriceRule`
5. `Create services for Room, SeatGroup, Seat, and PriceRule`
6. `Create endpoints for Room, SeatGroup, Seat, and PriceRule`
7. `Register services and endpoints in Program.cs`
8. `Add migration for RoomType and SeatStatus fields`
9. `Add wwwroot folder to fix DirectoryNotFoundException`
10. `Add AllowAnonymous attribute to Cinema & Seating endpoints for testing`
11. `Add API test file for Cinema & Seating endpoints`

## Notes

- All endpoints are currently set to `[AllowAnonymous]` for easy testing
- Remember to apply proper authorization before production use
- Database migration needs to be run before testing
- The application needs to be restarted after pulling these changes

