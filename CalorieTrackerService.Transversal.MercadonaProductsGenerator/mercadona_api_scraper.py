# -*- coding: utf-8 -*-
import requests
import json
import time
import os
import re  # Mueve este aquí
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
        """Obtener todas las categorías desde la API"""
        print("Obteniendo categorías desde la API...")
        
        url = f"{self.base_url}/categories/"
        response = requests.get(url, headers=self.headers)
        
        if response.status_code == 200:
            data = response.json()
            self.categories_data = data.get('results', [])
            
            # Guardar categorías (Dejamos esta línea, aunque el usuario no la pidió, es un buen punto de control)
            self.save_json(self.categories_data, 'mercadona_categories.json')
            
            print(f"✅ Obtenidas {len(self.categories_data)} categorías principales")
            return self.categories_data
        else:
            print(f"❌ Error al obtener categorías: {response.status_code}")
            return []
    
    def get_category_products(self, category_id, category_name):
        """Obtener productos de una categoría específica"""
        print(f"  Obteniendo productos de: {category_name} (ID: {category_id})")
        
        url = f"{self.base_url}/categories/{category_id}/"
        
        try:
            response = requests.get(url, headers=self.headers, timeout=30)
            
            if response.status_code == 200:
                data = response.json()
                
                products = self.extract_products_from_response(data)
                
                if products:
                    print(f"    ✅ {len(products)} productos encontrados")
                    return products
                else:
                    print(f"    ⚠️  No se encontraron productos en la respuesta")
                    return []
            else:
                print(f"    ❌ Error HTTP {response.status_code} para categoría {category_id}")
                return []
                
        except Exception as e:
            print(f"    ❌ Error obteniendo categoría {category_id}: {e}")
            return []
    
    def extract_products_from_response(self, data):
        """Extraer productos de la respuesta de la API"""
        products = []
        
        # Mercadona tiene una estructura anidada: categorías -> subcategorías -> productos
        def search_products(obj):
            if isinstance(obj, dict):
                # Buscar clave 'products'
                if 'products' in obj and isinstance(obj['products'], list):
                    for product in obj['products']:
                        if isinstance(product, dict):
                            # Normalizar producto
                            normalized = self.normalize_product(product)
                            if normalized:
                                products.append(normalized)
                
                # Buscar recursivamente en otras claves
                for key, value in obj.items():
                    if key != 'products':  # Evitar duplicados
                        search_products(value)
                        
            elif isinstance(obj, list):
                for item in obj:
                    search_products(item)
        
        search_products(data)
        return products
    
    def normalize_product(self, product_data):
        """Normalizar la estructura del producto"""
        try:
            # Información básica
            product_id = product_data.get('id', '')
            name = product_data.get('display_name', product_data.get('name', ''))
            price = product_data.get('price_instructions', {})
            
            # Extraer precio
            price_value = ''
            unit_price = ''
            
            if price:
                price_value = price.get('unit_price', '')
                if not price_value:
                    price_value = price.get('bulk_price', '')
                unit_price = price.get('unit_size', '')
            
            # Imagen
            thumbnail = product_data.get('thumbnail', '')
            if thumbnail and not thumbnail.startswith('http'):
                thumbnail = f"https://prod-mercadona.imgix.net{thumbnail}"
            
            # Marca
            brand = product_data.get('brand', '')
            if not brand and 'labels' in product_data:
                brand = product_data['labels'].get('brand', '')
            
            # Formatear producto
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
        """Scrapear todos los productos usando la API"""
        print("🚀 Iniciando scraping de productos Mercadona...")
        print("=" * 60)
        
        start_time = time.time()
        
        # 1. Obtener todas las categorías
        categories = self.get_all_categories()
        
        if not categories:
            print("No se pudieron obtener categorías. Saliendo...")
            return []
        
        total_categories = 0
        total_products = 0
        
        # 2. Recorrer cada categoría principal
        for main_category in categories:
            main_cat_name = main_category.get('name', 'Desconocida')
            main_cat_id = main_category.get('id', '')
            
            print(f"\n📁 Categoría principal: {main_cat_name}")
            
            # 3. Recorrer subcategorías
            subcategories = main_category.get('categories', [])
            
            for subcategory in subcategories:
                subcat_id = subcategory.get('id', '')
                subcat_name = subcategory.get('name', '')
                
                if subcat_id:  # Solo si tiene ID
                    # Obtener productos de esta subcategoría
                    products = self.get_category_products(subcat_id, subcat_name)
                    
                    if products:
                        # Añadir información de categoría a cada producto
                        for product in products:
                            product['main_category_id'] = main_cat_id
                            product['main_category_name'] = main_cat_name
                            product['subcategory_id'] = subcat_id
                            product['subcategory_name'] = subcat_name
                        
                        self.all_products.extend(products)
                        total_products += len(products)
                    
                    total_categories += 1
                    
                    # Esperar para no saturar la API
                    time.sleep(0.5)
        
        # 3. Guardar resultados
        self.save_results()
        
        end_time = time.time()
        elapsed = end_time - start_time
        
        print(f"\n{'='*60}")
        print("🎯 SCRAPING COMPLETADO")
        print(f"{'='*60}")
        print(f"⏱️  Tiempo total: {elapsed:.2f} segundos")
        print(f"📂 Categorías procesadas: {total_categories}")
        print(f"🛒 Productos obtenidos: {len(self.all_products)}")
        print(f"💾 Archivos guardados en la carpeta 'data/'")
        
        return self.all_products
    
    def save_json(self, data, filename):
        """Guardar datos en JSON"""
        os.makedirs('data', exist_ok=True)
        
        filepath = f"data/{filename}"
        with open(filepath, 'w', encoding='utf-8') as f:
            json.dump(data, f, ensure_ascii=False, indent=2)
        
        print(f"  💾 Guardado: {filepath}")
    
    def save_results(self):
        """Guardar solo el JSON completo con metadatos y el resumen de categorías (Modificado)"""
        if not self.all_products:
            print("No hay productos para guardar")
            return
        
        timestamp = datetime.now().strftime('%Y%m%d_%H%M%S')
        
        print("\n💾 Guardando resultados...")

        # 1. JSON completo con metadatos (mercadona_products_full_{timestamp}.json)
        full_data = {
            'metadata': {
                'scraped_at': datetime.now().isoformat(),
                'total_products': len(self.all_products),
                'source': 'Mercadona API',
                'url': 'https://tienda.mercadona.es'
            },
            'products': self.all_products
        }
        
        self.save_json(full_data, f'mercadona_products_full.json')
        
        # Se omiten la línea 2 (JSON solo productos) y la línea 3 (CSV) de la versión original.

        # 4. Resumen por categoría (mercadona_summary.json)
        self.save_category_summary()
    
    def save_category_summary(self):
        """Guardar resumen por categoría"""
        
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
        
        # Imprimir resumen
        print("\n📊 RESUMEN POR CATEGORÍA:")
        for category, count in sorted(category_summary.items(), key=lambda x: x[1], reverse=True):
            print(f"  {category}: {count} productos")
        
        print(f"\n🏷️  Total marcas únicas: {len(brand_summary)}")

# Script de ejecución rápido (No modificado, pero incluido por completitud)
def quick_scrape():
    """Scraper rápido para probar"""
    import requests
    import re # Necesario para re.findall
    
    print("🔍 Probando endpoints de la API...")
    
    # Endpoint principal
    url = "https://tienda.mercadona.es/api/v1_1/categories/"
    
    headers = {
        'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36'
    }
    
    response = requests.get(url, headers=headers)
    
    if response.status_code == 200:
        print("✅ API accesible")
        
        # Probar una categoría específica
        test_category_id = 112  # Aceite, vinagre y sal
        test_url = f"https://tienda.mercadona.es/api/v1_1/categories/{test_category_id}/"
        
        test_response = requests.get(test_url, headers=headers)
        
        if test_response.status_code == 200:
            test_data = test_response.json()
            
            # Guardar ejemplo
            os.makedirs('data', exist_ok=True)
            with open('data/api_example.json', 'w', encoding='utf-8') as f:
                json.dump(test_data, f, ensure_ascii=False, indent=2)
            
            print("✅ Ejemplo de categoría guardado en 'data/api_example.json'")
            
            # Contar productos en el ejemplo
            json_str = json.dumps(test_data)
            product_count = len(re.findall(r'"id"\s*:', json_str))
            
            print(f"📊 Aproximadamente {product_count} productos en la categoría de prueba")
            
            return True
        else:
            print(f"❌ No se pudo acceder a categoría específica: {test_response.status_code}")
            return False
    else:
        print(f"❌ No se pudo acceder a la API: {response.status_code}")
        return False

# Ejecutar (No modificado)
# ... (mantén el resto de la clase MercadonaAPIScraper igual)

if __name__ == "__main__":
    print("""
    ============================================
    🛒 SCRAPER DE MERCADONA - EJECUCIÓN AUTOMÁTICA
    ============================================
    """)
    
    # Iniciamos el scraper directamente sin preguntar
    scraper = MercadonaAPIScraper()
    
    try:
        products = scraper.scrape_all_products()
        
        # Estadísticas finales
        if products:
            print(f"\n📈 Estadísticas finales:")
            print(f"   Primer producto: {products[0]['name'][:50]}...")
            print(f"   Último producto: {products[-1]['name'][:50]}...")
            
            # Productos con imágenes
            with_images = sum(1 for p in products if p.get('image_url'))
            print(f"   Productos con imagen: {with_images} ({with_images/len(products)*100:.1f}%)")
            
            # Precios promedio
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
                    print(f"   Precio promedio: {sum(prices)/len(prices):.2f}€")
                    print(f"   Rango de precios: {min(prices):.2f}€ - {max(prices):.2f}€")
            except:
                pass
            
    except KeyboardInterrupt:
        print("\n⏹️ Scraping interrumpido por el usuario")
        if hasattr(scraper, 'all_products') and scraper.all_products:
            print(f"💾 Guardando {len(scraper.all_products)} productos obtenidos...")
            scraper.save_results()
    except Exception as e:
        print(f"❌ Error durante el scraping: {e}")