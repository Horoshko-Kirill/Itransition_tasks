const tbody = document.getElementById("tbody");
const loader = document.getElementById("loader");
let offset = 0;
const limit = 20;
let loading = false;
let allDone = false;
let expandedRow = null;


const translations = {
  en: {
    language: "Language",
    seed: "Seed",
    likes: "Likes",
    reviews: "Reviews (0-20)",
    id: "Id",
    isbn: "ISBN",
    title: "Title",
    authors: "Author(s)",
    publisher: "Publisher",
    loading: "Loading...",
    noMore: "No more books",
    error: "Error loading",
    authorsDetail: "Authors:",
    publisherDetail: "Publisher:",
    likesDetail: "Likes:",
    reviewsDetail: "Reviews",
    noReviews: "No reviews",
    randomSeed: "Generate random seed"
  },
  ru: {
    language: "Язык",
    seed: "Сид",
    likes: "Лайки",
    reviews: "Отзывы (0-20)",
    id: "ID",
    isbn: "ISBN",
    title: "Название",
    authors: "Автор(ы)",
    publisher: "Издатель",
    loading: "Загрузка...",
    noMore: "Книг больше нет",
    error: "Ошибка загрузки",
    authorsDetail: "Авторы:",
    publisherDetail: "Издатель:",
    likesDetail: "Лайки:",
    reviewsDetail: "Отзывы",
    noReviews: "Нет отзывов",
    randomSeed: "Случайный сид"
  },
  ja: {
    language: "言語",
    seed: "シード",
    likes: "いいね",
    reviews: "レビュー (0-20)",
    id: "ID",
    isbn: "ISBN",
    title: "タイトル",
    authors: "著者",
    publisher: "出版社",
    loading: "読み込み中...",
    noMore: "これ以上の本はありません",
    error: "読み込みエラー",
    authorsDetail: "著者:",
    publisherDetail: "出版社:",
    likesDetail: "いいね:",
    reviewsDetail: "レビュー",
    noReviews: "レビューはありません",
    randomSeed: "ランダムシード生成"
  },
  fr: {
    language: "Langue",
    seed: "Graine",
    likes: "J'aime",
    reviews: "Avis (0-20)",
    id: "Id",
    isbn: "ISBN",
    title: "Titre",
    authors: "Auteur(s)",
    publisher: "Éditeur",
    loading: "Chargement...",
    noMore: "Plus de livres",
    error: "Erreur de chargement",
    authorsDetail: "Auteurs:",
    publisherDetail: "Éditeur:",
    likesDetail: "J'aime:",
    reviewsDetail: "Avis",
    noReviews: "Pas d'avis",
    randomSeed: "Graine aléatoire"
  }
};

function updateTranslations(lang) {
  const t = translations[lang] || translations.en;

  document.getElementById("label-language").textContent = t.language;
  document.getElementById("label-seed").textContent = t.seed;
  document.getElementById("label-likes").textContent = t.likes;
  document.getElementById("label-reviews").textContent = t.reviews;
  document.getElementById("th-id").textContent = t.id;
  document.getElementById("th-isbn").textContent = t.isbn;
  document.getElementById("th-title").textContent = t.title;
  document.getElementById("th-authors").textContent = t.authors;
  document.getElementById("th-publisher").textContent = t.publisher;
  document.getElementById("random-seed").title = t.randomSeed;
}

function getParams() {
  const lang = document.getElementById("lang").value;
  return {
    seed: encodeURIComponent(document.getElementById("seed").value),
    lang: lang,
    likes: parseFloat(document.getElementById("likes").value),
    reviews: parseFloat(document.getElementById("reviews").value)
  };
}

function escapeHtml(s) {
  if (!s) return "";
  return s.replaceAll("&","&amp;").replaceAll("<","&lt;").replaceAll(">","&gt;").replaceAll('"',"&quot;");
}

function renderDetail(book) {
  const lang = document.getElementById("lang").value;
  const t = translations[lang] || translations.en;
  const template = document.getElementById("detail-template");
  const clone = template.content.cloneNode(true);
  
  clone.querySelector(".cover img").src = book.picture || book.coverImageBase64 || "";
  clone.querySelector(".title").textContent = book.title;
  clone.querySelector(".authors").textContent = (book.authors || []).join(", ");
  clone.querySelector(".publisher").textContent = book.publisher;
  clone.querySelector(".likes-value").textContent = book.likes || 0;
  

  clone.querySelector("#label-authors-detail").textContent = t.authorsDetail;
  clone.querySelector("#label-publisher-detail").textContent = t.publisherDetail;
  clone.querySelector(".label-likes-detail").textContent = t.likesDetail;
  clone.querySelector("#label-reviews-detail").textContent = t.reviewsDetail;
  
  const reviewsContainer = clone.querySelector(".list");
  if ((book.reviews || []).length === 0) {
    reviewsContainer.innerHTML = `<div>${t.noReviews}</div>`;
  } else {
    for (let r of book.reviews) {
      const div = document.createElement("div");
      div.className = "review";
      div.innerHTML = `<strong>${escapeHtml(r.author)}</strong>: <em>${escapeHtml(r.description)}</em>`;
      reviewsContainer.appendChild(div);
    }
  }
  return clone;
}

function makeBookRow(book) {
  const tr = document.createElement("tr");
  tr.classList.add("expandable");
  tr.innerHTML = `
    <td>${book.id ?? book.index}</td>
    <td>${escapeHtml(book.isbn)}</td>
    <td>${escapeHtml(book.title)}</td>
    <td>${escapeHtml((book.authors||[]).join(", "))}</td>
    <td>${escapeHtml(book.publisher)}</td>
  `;
  const detailRow = document.createElement("tr");
  const detailCell = document.createElement("td");
  detailCell.colSpan = 5;
  detailRow.appendChild(detailCell);
  detailRow.style.display = 'none';

  tr.addEventListener("click", () => {
    const isExpanded = tr.classList.contains("expanded");
    

    document.querySelectorAll("tr.expanded").forEach(row => {
      row.classList.remove("expanded");
      if (row._detailRow) {
        row._detailRow.style.display = 'none';
      }
    });
    

    if (!isExpanded) {
      detailCell.innerHTML = "";
      detailCell.appendChild(renderDetail(book));
      detailRow.style.display = '';
      tr.classList.add("expanded");
      tr._detailRow = detailRow;
    }
  });

  return [tr, detailRow];
}

async function loadMore() {
  if (loading || allDone) return;
  loading = true;
  
  const lang = document.getElementById("lang").value;
  const t = translations[lang] || translations.en;
  loader.textContent = t.loading;
  
  const { seed, lang: reqLang, likes, reviews } = getParams();
  const url = `/api/books?seed=${seed}&lang=${reqLang}&offset=${offset}&limit=${limit}&reviews=${reviews}&likes=${likes}`;
  
  try {
    const res = await fetch(url);
     if (!res.ok) {
      const text = await res.text();    // читаем ответ как текст (HTML)
  console.log('Ответ сервера:', text);  // выводим весь HTML в консоль
    // Если ответ с ошибкой, читаем тело как текст
    const errorText = await res.text();
    console.error('Ошибка запроса:', res.status, res.statusText);
    console.error('Тело ответа:', errorText);
    throw new Error(`fetch failed with status ${res.status}`);
  }
    const payload = await res.json();
    const items = payload.items || [];
    
    if (items.length === 0) {
      allDone = true;
      loader.textContent = t.noMore;
      return;
    }
    
    for (let book of items) {
      const [row, detail] = makeBookRow(book);
      tbody.appendChild(row);
      tbody.appendChild(detail);
    }
    
    offset += items.length;
    loading = false;
    loader.textContent = items.length === limit ? "" : t.noMore;
  } catch (e) {
    console.error(e);
    loader.textContent = t.error;
    loading = false;
  }
}

function reset() {
  offset = 0;
  allDone = false;
  tbody.innerHTML = "";
  loadMore();
}

function generateRandomSeed() {
  const randomSeed = Math.floor(Math.random() * 1000000); // 0-999999
  document.getElementById("seed").value = randomSeed;
  reset();
}

function validateReviewsInput(e) {
  const input = e.target;
  let value = parseFloat(input.value);
  
  if (isNaN(value)) {
    input.value = 3.0;
    return;
  }
  
  if (value < 0) {
    input.value = 0;
  } else if (value > 20) {
    input.value = 20;
  } else {
    input.value = value.toFixed(1);
  }
  
  reset();
}


document.getElementById("likes").addEventListener("input", e => {
  const val = e.target.value;
  const span = document.getElementById("likesVal");
  if (span) span.textContent = val;
  reset();
});

document.getElementById("reviews").addEventListener("input", validateReviewsInput);
document.getElementById("reviews").addEventListener("change", validateReviewsInput);
document.getElementById("reviews").addEventListener("blur", validateReviewsInput);

document.getElementById("lang").addEventListener("change", () => {
  updateTranslations(document.getElementById("lang").value);
  reset();
});

document.getElementById("seed").addEventListener("change", reset);
document.getElementById("likes").addEventListener("change", reset);

document.getElementById("random-seed").addEventListener("click", generateRandomSeed);


let isScrolling = false;
window.addEventListener("scroll", () => {
  if (isScrolling) return;
  isScrolling = true;
  
  setTimeout(() => {
    const scrollPosition = window.innerHeight + window.scrollY;
    const pageHeight = document.body.offsetHeight;
    
    if (scrollPosition >= pageHeight - 500 && !loading && !allDone) {
      loadMore();
    }
    isScrolling = false;
  }, 200);
});


updateTranslations(document.getElementById("lang").value);
reset();