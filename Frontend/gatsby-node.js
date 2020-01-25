const path = require(`path`);
const { createFilePath } = require(`gatsby-source-filesystem`);

// Add environment variables to process.env
require("dotenv").config({
  path: `../env_frontend`,
})

exports.createPages = async ({ graphql, actions }) => {
  const { createPage, createRedirect } = actions;

  // Project details page
  createPage({
    path: '/projects',
    matchPath: '/projects/:slug/:projectId',
    component: path.resolve('src/projectdetails/details.tsx')
  });

  // Create blog pages
  const blogPostTemplate = path.resolve('src/templates/blog-post.tsx');

  const blogposts = await graphql(`
    {
      allMarkdownRemark(filter: {frontmatter: {type: {eq: "blogs"}}}) {
        edges {
          node {
            frontmatter {
              type
            }
            fields {
              slug
            }
          }
        }
      }
    }
  `);

  if (blogposts.errors) {
    reporter.panicOnBuild('Error while running blog post GraphQL query.');
    return;
  }

  blogposts.data.allMarkdownRemark.edges
    .map(e => e.node)
    .forEach(blog => {
      const path = `/${blog.frontmatter.type}${blog.fields.slug}`;
      createPage({
        path,
        component: blogPostTemplate,
        context: {
          slug: blog.fields.slug
        }
      });
    });
};

exports.onCreateNode = ({ node, actions, getNode }) => {
  const { createNodeField } = actions;

  if (node.internal.type === `MarkdownRemark`) {
    const slug = createFilePath({
      node,
      getNode
    });
    createNodeField({
      name: 'slug',
      node,
      value: slug
    });
  }
};
