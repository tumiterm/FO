(function () {
    const config = {
        endpoint:
            window.__notificationsEndpoint ||
            document.querySelector('meta[name="notification-endpoint"]')?.content ||
            '/Notification/Index',
        cacheBust: true,
        alwaysShow: true,
        autoAdvance: true,
        globalSlideIntervalMs: 6000
    };

    const host = document.getElementById('dynamicNotificationHost');
    if (!host) return;

    const url = config.cacheBust ? appendCacheBust(config.endpoint) : config.endpoint;

    fetch(url, { headers: { 'Accept': 'application/json' } })
        .then(r => r.ok ? r.json() : Promise.reject(new Error('Notification fetch failed: ' + r.status)))
        .then(events => {
            if (!Array.isArray(events) || events.length === 0) return;

            const normalized = events.map(ev => ({
                id: ev.id ?? ev.Id,
                title: ev.title ?? ev.Title,
                headerIconCss: ev.headerIconCss ?? ev.HeaderIconCss,
                headerGradientCss: ev.headerGradientCss ?? ev.HeaderGradientCss,
                headerTextColor: ev.headerTextColor ?? ev.HeaderTextColor,
                imageUrl: ev.imageUrl ?? ev.ImageUrl,
                sizeClass: ev.sizeClass ?? ev.SizeClass ?? 'modal-lg',
                blocks: (ev.blocks ?? ev.Blocks ?? [])
                    .map(b => normalizeBlock(b))
                    .sort((a, b) => a.Order - b.Order)
            }));

            normalized.forEach(n => {
                const images = n.blocks.filter(b => b.Type === 5);
                const others = n.blocks.filter(b => b.Type !== 5);
                if (images.length) {
                    n.blocks = [...images, ...others];
                }
            });

            normalized.forEach(e => {
                if (!e.blocks.length) {
                    e.blocks = [{ Type: 0, Text: '(No content blocks attached to this notification yet.)' }];
                }
            });

            if (!normalized.length) return;

            const modalId = 'dynamicNotificationsModal';
            const useCarousel = normalized.length > 1;
            host.innerHTML = buildModalHtml(modalId, normalized, useCarousel);
            wireUp(modalId, useCarousel, normalized);
        })
        .catch(err => console.error(err));

    // ---- Helpers ---------------------------------------------------

    function normalizeBlock(b) {
        // Raw enum value (number or string name)
        const typeRaw = b.Type ?? b.type;
        const type = mapBlockType(typeRaw);

        // Handle list items supplied either as:
        // 1) Single comma-separated string  "Item A, Item B, Item C"
        // 2) Array of strings               ["Item A","Item B","Item C"]
        // 3) Already empty / null
        const rawList = b.ListItems ?? b.listItems;
        let listItems = [];

        if (typeof rawList === 'string') {
            listItems = rawList
                .split(',')
                .map(x => x.trim())
                .filter(x => x.length > 0);
        } else if (Array.isArray(rawList)) {
            listItems = rawList
                .map(x => (x || '').trim())
                .filter(x => x.length > 0);
        }

        // Normalize image URL (allow http(s), protocol-relative //, root-relative /, or bare domain -> prepend https)
        const rawImage = (b.ImageUrl ?? b.imageUrl ?? '').trim();
        let imageUrl = '';
        if (rawImage) {
            if (/^(https?:\/\/|\/\/|\/)/i.test(rawImage)) {
                imageUrl = rawImage;
            } else if (rawImage.includes('.') && !rawImage.includes(' ')) {
                imageUrl = 'https://' + rawImage;
            } else {
                imageUrl = rawImage; // leave as-is (could be relative segment)
            }
            // Reject dangerous schemes
            if (/^(javascript:|data:)/i.test(imageUrl)) {
                imageUrl = '';
            }
        }

        return {
            Type: type,
            Text: b.Text ?? b.text ?? '',
            ListItems: listItems,
            TableJson: b.TableJson ?? b.tableJson ?? '',
            ImageUrl: imageUrl,
            AltText: b.AltText ?? b.altText ?? '',
            Order: (b.Order ?? b.order ?? 0)
        };
    }

    function mapBlockType(val) {
        if (val === null || val === undefined) return 0;
        if (typeof val === 'number') return val;
        // Map enum names → numeric codes
        switch (val) {
            case 'Paragraph': return 0;
            case 'Html': return 1;
            case 'UnorderedList': return 2;
            case 'OrderedList': return 3;
            case 'Table': return 4;
            case 'Image': return 5;
            default: return 0;
        }
    }

    function appendCacheBust(endpoint) {
        const sep = endpoint.includes('?') ? '&' : '?';
        return `${endpoint}${sep}t=${Date.now()}`;
    }

    function buildModalHtml(id, events, carousel) {
        const sizeClass = events[0].sizeClass || 'modal-lg';
        return `
<div class="modal fade" id="${id}" tabindex="-1" aria-hidden="true" data-bs-backdrop="static">
  <div class="modal-dialog ${sizeClass} modal-dialog-centered">
    <div class="modal-content border-0">
      ${carousel ? buildCarousel(events) : buildSingle(events[0])}
      <div class="modal-footer border-0">
        <button type="button" class="btn-act" data-bs-dismiss="modal"><i class="fa fa-xmark"></i> Close</button>
        <button type="button" class="btn-act btn-primary" style="background:var(--ap-grad);color:#fff;border-color:var(--ap-red-strong);">
          <i class="fa fa-thumbs-up"></i> Continue
        </button>
      </div>
    </div>
  </div>
</div>`;
    }

    function buildSingle(e) {
        return `${buildHeader(e)}
<div class="modal-body" style="max-height:70vh;overflow-y:auto;">
  ${buildEventBody(e)}
</div>`;
    }

    function buildHeader(e) {
        const headerGradient = e.headerGradientCss || 'var(--ap-grad)';
        const textColor = e.headerTextColor || '#fff';
        return `
<div class="modal-header" style="background:${headerGradient};color:${textColor};">
  <h5 class="modal-title">
    ${e.headerIconCss ? `<i class="${e.headerIconCss} me-2"></i>` : ''}
    ${escapeHtml(e.title)}
  </h5>
  <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
</div>`;
    }

    function buildCarousel(events) {
        const indicators = events.map((_, i) => `
<button type="button" data-bs-target="#carouselNotif" data-bs-slide-to="${i}" ${i === 0 ? 'class="active" aria-current="true"' : ''} aria-label="Slide ${i + 1}"></button>`
        ).join('');

        const intervalAttr = config.autoAdvance
            ? ` data-bs-interval="${config.globalSlideIntervalMs}" data-bs-ride="carousel"`
            : '';

        const items = events.map((e, i) => `
<div class="carousel-item ${i === 0 ? 'active' : ''}">
  ${buildHeader(e)}
  <div class="modal-body" style="max-height:70vh;overflow-y:auto;">
    ${buildEventBody(e)}
  </div>
</div>`).join('');

        return `
<div id="carouselNotif" class="carousel slide"${intervalAttr}>
  <div class="carousel-indicators">${indicators}</div>
  <div class="carousel-inner">${items}</div>
  <button class="carousel-control-prev" type="button" data-bs-target="#carouselNotif" data-bs-slide="prev">
    <span class="carousel-control-prev-icon"></span>
    <span class="visually-hidden">Previous</span>
  </button>
  <button class="carousel-control-next" type="button" data-bs-target="#carouselNotif" data-bs-slide="next">
    <span class="carousel-control-next-icon"></span>
    <span class="visually-hidden">Next</span>
  </button>
</div>`;
    }

    function buildEventBody(e) {
        const img = e.imageUrl
            ? `<div class="text-center mb-3">
                 <img src="${encodeURI(e.imageUrl)}" alt="" style="max-height:160px;border-radius:16px;box-shadow:0 6px 18px -8px rgba(0,0,0,.25);" />
               </div>`
            : '';
        const blocksHtml = e.blocks.map(b => renderBlock(b)).join('');
        return img + blocksHtml;
    }

    function renderBlock(b) {
        switch (b.Type) {
            case 0: return `<p class="notif-paragraph">${safeBlockHtml(b.Text)}</p>`;
            case 1: return `<div class="notif-html">${safeBlockHtml(b.Text)}</div>`;
            case 2: return `<ul class="notif-list">${(b.ListItems || []).map(li => `<li>${escapeHtml(li || '')}</li>`).join('')}</ul>`;
            case 3: return `<ol class="notif-list">${(b.ListItems || []).map(li => `<li>${escapeHtml(li || '')}</li>`).join('')}</ol>`;
            case 4:
                try {
                    const parsed = JSON.parse(b.TableJson || '{}');
                    const headers = (parsed.headers || []).map(h => `<th>${escapeHtml(h)}</th>`).join('');
                    const rows = (parsed.rows || []).map(r => `<tr>${r.map(c => `<td>${escapeHtml(c)}</td>`).join('')}</tr>`).join('');
                    return `<div class="table-responsive"><table class="table table-sm table-bordered notif-table">
                              ${headers ? `<thead><tr>${headers}</tr></thead>` : ''}
                              <tbody>${rows}</tbody>
                            </table></div>`;
                } catch { return ''; }
            case 5:
                if (!b.ImageUrl) return '';
                return `<div class="text-center my-3">
                          <img src="${encodeURI(b.ImageUrl)}" alt="${escapeHtml(b.AltText || '')}" style="max-height:160px;border-radius:12px;" />
                        </div>`;
            default: return '';
        }
    }

    function safeBlockHtml(raw) {
        if (!raw) return '';
        return raw
            .replace(/<script[\s\S]*?>[\s\S]*?<\/script>/gi, '')
            .replace(/<style[\s\S]*?>[\s\S]*?<\/style>/gi, '');
    }

    function escapeHtml(str) {
        return (str || '').replace(/[&<>"']/g, s => ({
            '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;'
        }[s]));
    }

    function wireUp(modalId) {
        const modalEl = document.getElementById(modalId);
        if (!modalEl) return;
        const modal = new bootstrap.Modal(modalEl);
        modal.show();
    }
})();