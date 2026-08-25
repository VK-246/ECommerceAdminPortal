API Specification

All endpoints (except login/register) require a valid JWT Bearer token in the Authorization header.
All responses are wrapped in a generic ApiResponse<T> envelope for frontend consistency:
{ "success": true, "message": "...", "data": { ... } }

Authentication Module

POST /api/auth/register - Registers a new admin user.

POST /api/auth/login - Authenticates user, returns JWT token.

Category Module

GET /api/categories - Retrieves all categories.

GET /api/categories/{id} - Retrieves a specific category.

POST /api/categories - Creates a new category.

PUT /api/categories/{id} - Updates a category.

DELETE /api/categories/{id} - Deletes a category (fails if products depend on it).

Product Module

GET /api/products?page=1&pageSize=10&categoryId=5 - Retrieves products with pagination and optional category filtering.

GET /api/products/{id} - Retrieves a specific product with its category details.

POST /api/products - Creates a new product.

PUT /api/products/{id} - Updates a product.

DELETE /api/products/{id} - Deletes a product.

AI Module (Phase 2)

POST /api/ai/generate-description

Body: { "productName": "...", "category": "...", "keywords": ["..."] }

Response: Returns an SEO-optimized string.

POST /api/ai/chat

Body: { "message": "..." }

Response: Returns AI marketing advice.