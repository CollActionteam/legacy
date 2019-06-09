const site = document.head.querySelector("meta[property='disqus:site']") as HTMLMetaElement;
const id = document.head.querySelector("meta[property='disqus:id']") as HTMLMetaElement;

if (site && id) {
  const disqus_config = function () {
    this.page.url = window.location.href;
    this.page.identifier = id.content;
  };

  (function() { // DON'T EDIT BELOW THIS LINE
    const d = document, s = d.createElement("script");
    s.src = `https://${site.content}.disqus.com/embed.js`;
    s.setAttribute("data-timestamp", new Date().toISOString());
    (d.head || d.body).appendChild(s);
  })();
}