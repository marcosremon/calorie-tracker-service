CREATE TABLE users (
    user_id SERIAL PRIMARY KEY,
    user_email TEXT NOT NULL UNIQUE,
    user_password BYTEA NOT NULL,
    user_name TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE products (
    product_id SERIAL PRIMARY KEY,
    product_name TEXT NOT NULL,
    brand TEXT,
    calories_100g FLOAT NOT NULL,
    carbs_100g FLOAT,
    fat_100g FLOAT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE consumption (
    consumption_id SERIAL PRIMARY KEY,

    user_id INT NOT NULL REFERENCES users(user_id) ON DELETE CASCADE,
    product_id INT NOT NULL REFERENCES products(product_id) ON DELETE CASCADE,

    grams FLOAT NOT NULL,
    state INTEGER NOT NULL,
    state_string TEXT NOT NULL,

    calories FLOAT,
    protein FLOAT,
    carbs FLOAT,
    fat FLOAT,

    date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE ai_logs (
    ai_logs_id SERIAL PRIMARY KEY,

    image_url TEXT,
    detected_text TEXT,
    detected_product TEXT,

    date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_users_email ON users(user_email);
CREATE INDEX idx_products_name ON products(product_name);
CREATE INDEX idx_consumption_user ON consumption(user_id);
CREATE INDEX idx_consumption_date ON consumption(date);
