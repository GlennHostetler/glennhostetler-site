// Small, dependency-free behavior for the site: mobile menu, the testimonial slider,
// and the pause control for the scrolling logo strip.
(function () {
  var reduced = window.matchMedia && window.matchMedia('(prefers-reduced-motion: reduce)').matches;

  // ---------- Mobile navigation ----------
  var toggle = document.querySelector('.nav-toggle');
  var nav = document.getElementById('site-nav');
  if (toggle && nav) {
    toggle.addEventListener('click', function () {
      var open = nav.classList.toggle('open');
      toggle.setAttribute('aria-expanded', open ? 'true' : 'false');
    });
    nav.addEventListener('click', function (e) {
      if (e.target.tagName === 'A') {
        nav.classList.remove('open');
        toggle.setAttribute('aria-expanded', 'false');
      }
    });
  }

  // ---------- Logo strip: pause button ----------
  var marquee = document.querySelector('[data-marquee]');
  var marqueeBtn = document.querySelector('[data-marquee-toggle]');
  if (marquee && marqueeBtn) {
    marqueeBtn.addEventListener('click', function () {
      var paused = marquee.classList.toggle('paused');
      marqueeBtn.textContent = paused ? 'Play logos' : 'Pause logos';
      marqueeBtn.setAttribute('aria-pressed', paused ? 'true' : 'false');
    });
  }

  // ---------- Testimonials: two at a time, sliding right to left ----------
  var region = document.querySelector('[data-quotes]');
  if (!region) return;
  var track = region.querySelector('.quotes-track');
  var originals = Array.prototype.slice.call(track.children);
  var total = originals.length;
  if (total < 3) return;

  // Clone the first slides onto the end so the loop never visibly jumps.
  for (var c = 0; c < 2; c++) {
    var clone = originals[c].cloneNode(true);
    clone.setAttribute('aria-hidden', 'true');
    track.appendChild(clone);
  }

  var INTERVAL = 4500;      // time each pair stays on screen
  var index = 0;
  var timer = null;
  var playing = !reduced;
  var hovering = false;
  var count = region.querySelector('.quote-count');
  var playBtn = region.querySelector('[data-quote-play]');

  function perView() {
    var v = parseInt(getComputedStyle(region).getPropertyValue('--per'), 10);
    return v > 0 ? v : 1;
  }

  function place(animate) {
    track.style.transition = animate && !reduced ? '' : 'none';
    track.style.transform = 'translateX(' + (-index * 100 / perView()) + '%)';
    if (!animate) { void track.offsetWidth; track.style.transition = ''; }
    if (count) count.textContent = ((index % total) + 1) + ' of ' + total;
  }

  var snapping = false;

  function next() {
    if (snapping) return;
    index += 1;
    place(true);
    if (index >= total) {
      // The view is now showing the cloned slides, which look identical to the
      // start. Once the slide finishes, quietly jump back to the real start.
      snapping = true;
      setTimeout(function () {
        index = 0;
        place(false);
        snapping = false;
      }, reduced ? 0 : 850);
    }
  }

  function prev() {
    if (snapping) return;
    if (index === 0) { index = total; place(false); }
    index -= 1;
    // Wait a frame so the browser registers the jump before animating.
    requestAnimationFrame(function () { place(true); });
  }

  function stop() { if (timer) { clearInterval(timer); timer = null; } }
  function start() {
    stop();
    if (playing && !hovering) timer = setInterval(next, INTERVAL);
  }

  function updatePlayLabel() {
    if (!playBtn) return;
    playBtn.textContent = playing ? 'Pause' : 'Play';
    playBtn.setAttribute('aria-label', playing ? 'Pause testimonials' : 'Play testimonials');
  }

  region.querySelector('[data-quote-prev]').addEventListener('click', function () { prev(); start(); });
  region.querySelector('[data-quote-next]').addEventListener('click', function () { next(); start(); });
  if (playBtn) {
    playBtn.addEventListener('click', function () { playing = !playing; updatePlayLabel(); start(); });
  }
  region.addEventListener('mouseenter', function () { hovering = true; stop(); });
  region.addEventListener('mouseleave', function () { hovering = false; start(); });
  region.addEventListener('focusin', stop);
  region.addEventListener('focusout', start);
  window.addEventListener('resize', function () { place(false); });

  place(false);
  updatePlayLabel();
  start();
})();
