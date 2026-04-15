INSERT INTO Venue (Name, Location, Capacity)
VALUES ('Grand Hall', 'Johannesburg', 500);

INSERT INTO Event (Name, Description, StartDate, EndDate)
VALUES ('Tech Expo', 'Annual tech event', '2026-05-01', '2026-05-03');

INSERT INTO Booking (VenueId, EventId, UniqueBookingCode)
VALUES (1, 1, 'BOOK001');


DELETE FROM Venue WHERE VenueId = 1;
