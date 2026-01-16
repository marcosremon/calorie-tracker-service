-- 1. Tabla de Usuarios
CREATE TABLE users (
    user_id BIGSERIAL PRIMARY KEY, -- Coincide con long en C#
    user_email TEXT NOT NULL UNIQUE,
    user_password BYTEA NOT NULL,
    user_name TEXT NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- 2. Tabla de Productos (General)
CREATE TABLE products (
    product_id BIGSERIAL PRIMARY KEY,
    product_name TEXT NOT NULL,
    brand TEXT NOT NULL,
    calories_100g REAL NOT NULL, -- REAL es el equivalente a float en PostgreSQL
    carbs_100g REAL NOT NULL,
    fat_100g REAL NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- 3. Tabla de Productos de Mercadona (Basada en tu nueva clase)
CREATE TABLE mercadona_products (
    id TEXT PRIMARY KEY,
    name TEXT NOT NULL,
    brand TEXT,
    description TEXT,
    price TEXT,
    unit_price DOUBLE PRECISION,
    category_id TEXT,
    category_name TEXT,
    image_url TEXT,
    published BOOLEAN,
    available_online BOOLEAN,
    share_url TEXT,
    scraped_at TEXT,
    main_category_id INT,
    main_category_name TEXT,
    subcategory_id INT,
    subcategory_name TEXT
);

-- 4. Tabla de Consumo
CREATE TABLE consumption (
    consumption_id BIGSERIAL PRIMARY KEY,
    user_id BIGINT NOT NULL REFERENCES users(user_id) ON DELETE CASCADE,
    product_id BIGINT NOT NULL REFERENCES products(product_id) ON DELETE CASCADE,
    grams REAL NOT NULL,
    state INTEGER NOT NULL,
    state_string TEXT NOT NULL,
    calories REAL NOT NULL,
    protein REAL NOT NULL,
    carbs REAL NOT NULL,
    fat REAL NOT NULL,
    date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- 5. Tabla de Logs de AI
CREATE TABLE ai_logs (
    ai_logs_id BIGSERIAL PRIMARY KEY,
    image_url TEXT NOT NULL,
    detected_text TEXT NOT NULL,
    detected_product TEXT NOT NULL,
    date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

---
--- Índices para optimización
---
CREATE INDEX idx_users_email ON users(user_email);
CREATE INDEX idx_products_name ON products(product_name);
CREATE INDEX idx_consumption_user ON consumption(user_id);
CREATE INDEX idx_consumption_date ON consumption(date);