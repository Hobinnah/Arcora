-- Clears the seeded listing domain so DbSeeder can repopulate it on next startup.
-- Deletes children before parents to respect foreign keys. Lookup tables
-- (Organization, RentalUnit, Property, Address, catalogs) are intentionally left
-- in place because DbSeeder skips them when they already exist.
SET NOCOUNT ON;

DELETE FROM [CalendarEvents];
DELETE FROM [Ratings];
DELETE FROM [Leases];
DELETE FROM [Tenants];
DELETE FROM [ListingPolicies];
DELETE FROM [ListingRules];
DELETE FROM [ListingAmenities];
DELETE FROM [ListingPhotos];
DELETE FROM [ListingAccessInstructions];
DELETE FROM [ListingTermPrices];
DELETE FROM [Listings];
DELETE FROM [OrganizationMembers];
