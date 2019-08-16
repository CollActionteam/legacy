const path = require(`path`);
const { createFilePath } = require(`gatsby-source-filesystem`);

exports.createPages = async ({ graphql, actions }) => {
  const { createPage } = actions;
  const blogPostTemplate = path.resolve('src/templates/blog-post.js');

  const posts = await graphql(`
    {
      allMarkdownRemark(sort: {order: DESC, fields: frontmatter___date}) {
        edges {
          node {
            frontmatter {
              title
              date
            }
            fields {
              slug
            }
            html
          }
        }
      }
    }
  `);

  if (posts.errors) {
    reporter.panicOnBuild('Error while running blog post GraphQL query.');
    return;
  }

  posts.data.allMarkdownRemark.edges.forEach(( { node: post }) => {
    createPage({
      path: post.fields.slug,
      component: blogPostTemplate,
      context: {
        slug: post.fields.slug
      }
    });
  });
};

exports.onCreateNode = ({ node, actions, getNode }) => {
  const { createNodeField } = actions;

  if (node.internal.type === `MarkdownRemark`) {
    const slug = createFilePath({
      node,
      getNode,
      basePath: 'blogs'
    });
    createNodeField({
      name: 'slug',
      node,
      value: `blogs${slug}`
    });
  }
};
