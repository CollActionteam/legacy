/*
 * Generates a sitemap on build. Since we deploy the sitemap on a CDN, 
 * we can't generate one dynamically. Given our site might be difficult 
 * to parse by google, we should have one anyway. This script runs through
 * nodejs, and generates a sitemap.xml in the /public folder, which gets
 * deployed to our CDN.
 *
 * NodeJS doesn't have a xml-dom library (except through external dependencies),
 * since this is pretty simple, I'm generating the sitemap XML through querying
 * the GraphQL API of the production site. We should be deploying regularly, 
 * so this is fine.
 */

const https = require('https');

const query = `
  query {
    crowdactions(status: ACTIVE) {
      displayPriority
      isClosed
      url
      cardImage {
        description
        url
      }
    }
  }
`;

process.stdout.write('<?xml version="1.0" encoding="UTF-8"?>');
process.stdout.write('<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9" xmlns:image="http://www.google.com/schemas/sitemap-image/1.1">');
process.stdout.write('<url><loc>https://collaction.org/</loc><changefreq>always</changefreq><priority>1.0</priority></url>');
process.stdout.write('<url><loc>https://collaction.org/privacy-policy</loc><changefreq>monthly</changefreq><priority>0.1</priority></url>');
process.stdout.write('<url><loc>https://collaction.org/about</loc><changefreq>monthly</changefreq><priority>0.3</priority></url>');
process.stdout.write('<url><loc>https://collaction.org/donate</loc><changefreq>monthly</changefreq><priority>0.3</priority></url>');
process.stdout.write('<url><loc>https://collaction.org/find</loc><changefreq>always</changefreq><priority>0.3</priority></url>');
https.get(`https://api.collaction.org/graphql?query=${encodeURIComponent(query)}`, (res) => {
  if (res.statusCode !== 200) {
    console.error(`GraphQL Error: ${res.statusCode}`);
    process.exit(1);
  }
  else {
    let body = '';
    res.on(
      'data', 
      chunk => {
        body += chunk.toString();
    });
    res.on(
        'end',
        () => {
          const crowdactions = JSON.parse(body).data.crowdactions;
          crowdactions.forEach(crowdaction => {
            const priority = crowdaction.displayPriority === 'TOP' ? 1.0 : (crowdaction.displayPriority === 'MEDIUM' ? 0.8 : 0.6);
            const changeFreq = crowdaction.isClosed ? 'monthly' : 'hourly';
            const cardImage = crowdaction.cardImage;
            const imageMap = cardImage ? `<image:image><image:loc>${cardImage.url}</image:loc><image:caption>${cardImage.description}</image:caption><image:title>${cardImage.description}</image:title></image:image>` : '';
            process.stdout.write(`<url><loc>https://collaction.org${crowdaction.url}</loc><changefreq>${changeFreq}</changefreq><priority>${priority}</priority>${imageMap}</url>`);
          });
          process.stdout.write('</urlset>');
        });
  }
});