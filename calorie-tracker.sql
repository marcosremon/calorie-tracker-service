CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    email TEXT NOT NULL UNIQUE,
    password TEXT NOT NULL,
    name TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE products (
    id SERIAL PRIMARY KEY,
    name TEXT NOT NULL,
    brand TEXT,
    calories_100g FLOAT NOT NULL,
    protein_100g FLOAT,
    carbs_100g FLOAT,
    fat_100g FLOAT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE consumption (
    id SERIAL PRIMARY KEY,
    user_id INT NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    product_id INT NOT NULL REFERENCES products(id) ON DELETE CASCADE,
    grams FLOAT NOT NULL,
    state TEXT NOT NULL, -- raw or cooked
    calories FLOAT,
    protein FLOAT,
    carbs FLOAT,
    fat FLOAT,
    date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE ai_logs (
    id SERIAL PRIMARY KEY,
    user_id INT REFERENCES users(id),
    image_url TEXT,
    detected_text TEXT,
    detected_product TEXT,
    date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_products_name ON products(name);
CREATE INDEX idx_consumption_user ON consumption(user_id);
CREATE INDEX idx_consumption_date ON consumption(date);
