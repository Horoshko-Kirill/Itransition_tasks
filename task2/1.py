
import hashlib
import zipfile
import os

EXTRACT_DIR = "../task2"
EMAIL = "khorosko15@gmail.com".lower()

hashes = []
for root, dirs, files in os.walk(EXTRACT_DIR):
    for filename in sorted(files):
        filepath = os.path.join(root, filename)
        with open(filepath, 'rb') as f:
            content = f.read()
            file_hash = hashlib.sha3_256(content).hexdigest()
            hashes.append(file_hash)


hashes.sort(reverse=True)  # сортировка по убыванию как строк
joined_hashes = ''.join(hashes)  # склеиваем без разделителей
final_string = joined_hashes + EMAIL

# === Вычисление финального SHA3-256 ===
final_hash = hashlib.sha3_256(final_string.encode('utf-8')).hexdigest()

print(f"Финальный SHA3-256 хеш:\n{final_hash}")