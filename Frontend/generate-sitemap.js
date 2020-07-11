/*
 * Generates a sitemap on build. Since we deploy the sitemap on a CDN, 
 * we can't generate one dynamically. Given our site might be difficult 
 * to parse by google, we should have one anyway. This script runs through
 * nodejs, and generates a sitemap.xml in the /public folder, which gets
 * deployed to our CDN.
 *
 * We're generating the sitemap XML through querying the GraphQL API of the 
 * production site, and then generating the XML from that. We should be 
 * deploying regularly, so this is a decent trade-off.
 * 
 * Unfortunately, nodejs doesn't have a build-in xml generator, but since this
 * is a pretty simple job, we're just outputting the xml manually.
 */

const https = require('https');

const query = `
  query {
    crowdactions(status: ACTIVE) {
      name
      displayPriority
      isClosed
      descriptionVideoLink
      url
      bannerImage {
        description
        url
      }
    }
  }
`;

const backend = process.env.SITEMAP_BACKEND_URL;
const frontend = process.env.URL;

process.stdout.write('<?xml version="1.0" encoding="UTF-8"?>\n');
process.stdout.write('<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9" xmlns:image="http://www.google.com/schemas/sitemap-image/1.1" xmlns:video="http://www.google.com/schemas/sitemap-video/1.1">');
process.stdout.write(`<url><loc>${frontend}</loc><changefreq>always</changefreq><priority>1.0</priority><video:video><video:title>Crowdacting</video:title><video:duration>208</video:duration><video:thumbnail_loc>https://img.youtube.com/vi/xnIJo91Gero/0.jpg</video:thumbnail_loc><video:description>What is crowdacting</video:description><video:player_loc allow_embed="yes">https://www.youtube.com/watch?v=xnIJo91Gero</video:player_loc></video:video></url>`);
process.stdout.write(`<url><loc>${frontend}/privacy-policy</loc><changefreq>monthly</changefreq><priority>0.1</priority></url>`);
process.stdout.write(`<url><loc>${frontend}/about</loc><changefreq>monthly</changefreq><priority>0.3</priority></url>`);
process.stdout.write(`<url><loc>${frontend}/donate</loc><changefreq>monthly</changefreq><priority>0.3</priority></url>`);
process.stdout.write(`<url><loc>${frontend}/find</loc><changefreq>always</changefreq><priority>0.3</priority></url>`);

const graphqlQuery = `${backend}/graphql?query=${encodeURIComponent(query)}`;
https.get(graphqlQuery, (res) => {
  if (res.statusCode !== 200) {
    console.error(`Unexpected HTTP Status: ${res.statusCode}`);
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
          const parsed = JSON.parse(body);
          if (parsed.errors) {
            console.error(`GraphQL Error: ${parsed.errors}`);
            process.exit(1);
          }
          const crowdactions = parsed.data.crowdactions;
          crowdactions.forEach(crowdaction => {
            const priority = crowdaction.displayPriority === 'TOP' ? 1.0 : (crowdaction.displayPriority === 'MEDIUM' ? 0.8 : 0.6);
            const changeFreq = crowdaction.isClosed ? 'monthly' : 'hourly';
            const bannerImage = crowdaction.bannerImage;
            const imageMap = bannerImage ? `<image:image><image:loc>${bannerImage.url}</image:loc><image:caption>${bannerImage.description}</image:caption><image:title>${bannerImage.description}</image:title></image:image>` : '';
            const video = crowdaction.descriptionVideoLink;
            const videoId = video ? (video.lastIndexOf('watch?v=') ? video.substring(video.lastIndexOf('=') + 1) : video.substring(video.lastIndexOf('/') + 1)) : null;
            const videoMap = video ? `<video:video><video:title>${crowdaction.name}</video:title><video:thumbnail_loc>https://img.youtube.com/vi/${videoId}/0.jpg</video:thumbnail_loc><video:description>${crowdaction.name}</video:description><video:player_loc allow_embed="yes">${video}</video:player_loc></video:video>` : '';
            process.stdout.write(`<url><loc>${frontend}${crowdaction.url}</loc><changefreq>${changeFreq}</changefreq><priority>${priority}</priority>${imageMap}${videoMap}</url>`);
          });
          process.stdout.write('</urlset>');
        });
  }
}).on('error', (err) => {
  console.error("Error: " + err.message);
  process.exit(1);
});;