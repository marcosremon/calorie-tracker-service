# -*- coding: utf-8 -*-
import requests
import json
import time
import os
from datetime import datetime
import csv
from collections import defaultdict

class MercadonaAPIScraper:
    def __init__(self):
        self.base_url = "https://tienda.mercadona.es/api/v1_1"
        self.headers = {
            'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36',
            'Accept': 'application/json',
            'Accept-Language': 'es-ES,es;q=0.9',
            'Referer': 'https://tienda.mercadona.es/',
        }
        self.all_products = []
        self.categories_data = []
        
    def get_all_categories(self):
        print("Obteniendo categorias desde la API...")
        
        url = f"{self.base_url}/categories/"
        response = requests.get(url, headers=self.headers)
        
        if response.status_code == 200:
            data = response.json()
            self.categories_data = data.get('results', [])
            
            # Guardar categorias
            self.save_json(self.categories_data, 'mercadona_categories.json')
            
            print(f"[OK] Obtenidas {len(self.categories_data)} categorias principales")
            return self.categories_data
        else:
            print(f"[ERROR] Error al obtener categorias: {response.status_code}")
            return []
    
    def get_category_products(self, category_id, category_name):
        print(f"  Obteniendo productos de: {category_name} (ID: {category_id})")
        
        url = f"{self.base_url}/categories/{category_id}/"
        
        try:
            response = requests.get(url, headers=self.headers, timeout=30)
            
            if response.status_code == 200:
                data = response.json()
                
                products = self.extract_products_from_response(data)
                
                if products:
                    print(f"    [OK] {len(products)} productos encontrados")
                    return products
                else:
                    print(f"    [AVISO] No se encontraron productos en la respuesta")
                    return []
            else:
                print(f"    [ERROR] Error HTTP {response.status_code} para categoria {category_id}")
                return []
                
        except Exception as e:
            print(f"    [ERROR] Error obteniendo categoria {category_id}: {e}")
            return []
    
    def extract_products_from_response(self, data):
        products = []
        
        def search_products(obj):
            if isinstance(obj, dict):
                if 'products' in obj and isinstance(obj['products'], list):
                    for product in obj['products']:
                        if isinstance(product, dict):
                            normalized = self.normalize_product(product)
                            if normalized:
                                products.append(normalized)
                
                for key, value in obj.items():
                    if key != 'products':
                        search_products(value)
                        
            elif isinstance(obj, list):
                for item in obj:
                    search_products(item)
        
        search_products(data)
        return products
    
    def normalize_product(self, product_data):
        try:
            product_id = product_data.get('id', '')
            name = product_data.get('display_name', product_data.get('name', ''))
            price = product_data.get('price_instructions', {})
            
            price_value = ''
            unit_price = ''
            
            if price:
                price_value = price.get('unit_price', '')
                if not price_value:
                    price_value = price.get('bulk_price', '')
                unit_price = price.get('unit_size', '')
            
            thumbnail = product_data.get('thumbnail', '')
            if thumbnail and not thumbnail.startswith('http'):
                thumbnail = f"https://prod-mercadona.imgix.net{thumbnail}"
            
            brand = product_data.get('brand', '')
            if not brand and 'labels' in product_data:
                brand = product_data['labels'].get('brand', '')
            
            product = {
                'id': product_id,
                'name': name,
                'brand': brand,
                'description': product_data.get('description', ''),
                'price': price_value,
                'unit_price': unit_price,
                'category_id': product_data.get('category_id', ''),
                'category_name': product_data.get('category_name', ''),
                'image_url': thumbnail,
                'published': product_data.get('published', False),
                'available_online': product_data.get('available_online', False),
                'share_url': product_data.get('share_url', ''),
                'scraped_at': datetime.now().isoformat()
            }
            
            return product
            
        except Exception as e:
            print(f"Error normalizando producto: {e}")
            return None
    
    def scrape_all_products(self):
        print("Iniciando scraping de productos Mercadona...")
        print("=" * 60)
        
        start_time = time.time()
        
        categories = self.get_all_categories()
        
        if not categories:
            print("No se pudieron obtener categorias. Saliendo...")
            return []
        
        total_categories = 0
        total_products = 0
        
        for main_category in categories:
            main_cat_name = main_category.get('name', 'Desconocida')
            main_cat_id = main_category.get('id', '')
            
            print(f"\n[CATEGORIA] {main_cat_name}")
            
            subcategories = main_category.get('categories', [])
            
            for subcategory in subcategories:
                subcat_id = subcategory.get('id', '')
                subcat_name = subcategory.get('name', '')
                
                if subcat_id:
                    products = self.get_category_products(subcat_id, subcat_name)
                    
                    if products:
                        for product in products:
                            product['main_category_id'] = main_cat_id
                            product['main_category_name'] = main_cat_name
                            product['subcategory_id'] = subcat_id
                            product['subcategory_name'] = subcat_name
                        
                        self.all_products.extend(products)
                        total_products += len(products)
                    
                    total_categories += 1
                    time.sleep(0.5)
        
        self.save_results()
        
        end_time = time.time()
        elapsed = end_time - start_time
        
        print(f"\n{'='*60}")
        print("SCRAPING COMPLETADO")
        print(f"{'='*60}")
        print(f"Tiempo total: {elapsed:.2f} segundos")
        print(f"Categorias procesadas: {total_categories}")
        print(f"Productos obtenidos: {len(self.all_products)}")
        print(f"Archivos guardados en la carpeta 'data/'")
        
        return self.all_products
    
    def save_json(self, data, filename):
        os.makedirs('data', exist_ok=True)
        
        filepath = f"data/{filename}"
        with open(filepath, 'w', encoding='utf-8') as f:
            json.dump(data, f, ensure_ascii=False, indent=2)
        
        print(f"  [GUARDADO] {filepath}")
    
    def save_results(self):
        if not self.all_products:
            print("No hay productos para guardar")
            return
        
        timestamp = datetime.now().strftime('%Y%m%d_%H%M%S')
        
        print("\nGuardando resultados...")

        full_data = {
            'metadata': {
                'scraped_at': datetime.now().isoformat(),
                'total_products': len(self.all_products),
                'source': 'Mercadona API',
                'url': 'https://tienda.mercadona.es'
            },
            'products': self.all_products
        }
        
        self.save_json(full_data, f'mercadona_products_full_{timestamp}.json')
        self.save_category_summary()
    
    def save_category_summary(self):
        
        category_summary = defaultdict(int)
        brand_summary = defaultdict(int)
        
        for product in self.all_products:
            cat_name = product.get('main_category_name', 'Desconocida')
            brand = product.get('brand', 'Sin marca')
            
            category_summary[cat_name] += 1
            if brand:
                brand_summary[brand] += 1
        
        summary = {
            'total_products': len(self.all_products),
            'products_by_category': dict(category_summary),
            'products_by_brand': dict(brand_summary),
            'unique_brands': len(brand_summary),
            'unique_categories': len(category_summary)
        }
        
        self.save_json(summary, 'mercadona_summary.json')
        
        print("\nRESUMEN POR CATEGORIA:")
        for category, count in sorted(category_summary.items(), key=lambda x: x[1], reverse=True):
            print(f"  {category}: {count} productos")
        
        print(f"\nTotal marcas unicas: {len(brand_summary)}")

def quick_scrape():
    import re
    
    print("Probando endpoints de la API...")
    
    url = "https://tienda.mercadona.es/api/v1_1/categories/"
    
    headers = {
        'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36'
    }
    
    response = requests.get(url, headers=headers)
    
    if response.status_code == 200:
        print("[OK] API accesible")
        
        test_category_id = 112
        test_url = f"https://tienda.mercadona.es/api/v1_1/categories/{test_category_id}/"
        
        test_response = requests.get(test_url, headers=headers)
        
        if test_response.status_code == 200:
            test_data = test_response.json()
            
            os.makedirs('data', exist_ok=True)
            with open('data/api_example.json', 'w', encoding='utf-8') as f:
                json.dump(test_data, f, ensure_ascii=False, indent=2)
            
            print("[GUARDADO] Ejemplo de categoria guardado en 'data/api_example.json'")
            
            json_str = json.dumps(test_data)
            product_count = len(re.findall(r'"id"\s*:', json_str))
            
            print(f"Aproximadamente {product_count} productos en la categoria de prueba")
            
            return True
        else:
            print(f"[ERROR] No se pudo acceder a categoria especifica: {test_response.status_code}")
            return False
    else:
        print(f"[ERROR] No se pudo acceder a la API: {response.status_code}")
        return False

if __name__ == "__main__":
    print("""
    ============================================
    SCRAPER DE MERCADONA - VERSION API
    ============================================
    
    Este scraper usa la API oficial de Mercadona para obtener
    todos los productos de forma rapida y estructurada.
    
    Opciones:
    1. Scrapeo completo (recomendado)
    2. Probar conexion con API
    3. Usar datos de categorias existentes
    """)
    
    choice = input("Selecciona opcion (1, 2 o 3): ").strip()
    
    if choice == "2":
        quick_scrape()
    elif choice == "3":
        try:
            with open('data/mercadona_categories.json', 'r', encoding='utf-8') as f:
                categories = json.load(f)
                print(f"Cargadas {len(categories)} categorias desde archivo")
        except FileNotFoundError:
            print("[ERROR] Archivo 'data/mercadona_categories.json' no encontrado. Ejecuta la opcion 1 primero.")
        except Exception as e:
            print(f"[ERROR] Error al cargar el archivo: {e}")
    else:
        scraper = MercadonaAPIScraper()
        try:
            products = scraper.scrape_all_products()
            
            if products:
                print(f"\nEstadisticas finales:")
                print(f"   Primer producto: {products[0]['name'][:50]}...")
                print(f"   Ultimo producto: {products[-1]['name'][:50]}...")
                
                with_images = sum(1 for p in products if p.get('image_url'))
                print(f"   Productos con imagen: {with_images} ({with_images/len(products)*100:.1f}%)")
                
                try:
                    prices = []
                    import re
                    for p in products:
                        price_str = p.get('price', '0')
                        if price_str and isinstance(price_str, str):
                            nums = re.findall(r'[\d,]+', price_str)
                            if nums:
                                price_num = float(nums[0].replace(',', '.'))
                                prices.append(price_num)
                    
                    if prices:
                        print(f"   Precio promedio: {sum(prices)/len(prices):.2f} EUR")
                        print(f"   Rango de precios: {min(prices):.2f} EUR - {max(prices):.2f} EUR")
                except:
                    pass
                
        except KeyboardInterrupt:
            print("\n[AVISO] Scraping interrumpido por el usuario")
            if hasattr(scraper, 'all_products') and scraper.all_products:
                print(f"Guardando {len(scraper.all_products)} productos obtenidos...")
                scraper.save_results()
        except Exception as e:
            print(f"[ERROR] Error durante el scraping: {e}")