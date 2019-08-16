const path = require(`path`);
const { createFilePath } = require(`gatsby-source-filesystem`);

exports.createPages = async ({ graphql, actions }) => {
  const { createPage } = actions;
  const blogPostTemplate = path.resolve('src/templates/blog-post.js');

  const blogposts = await graphql(`
    {
      allFile(filter: {sourceInstanceName: {eq: "blogs"}}) {
        edges {
          node {
            childMarkdownRemark {
              fields {
                slug
              }
              html
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

  blogposts.data.allFile.edges
    .map(blog => blog.node)
    .filter(blog => blog.childMarkdownRemark !== null)
    .map(blog => blog.childMarkdownRemark)
    .forEach(blog => {
      const path = `blogs${blog.fields.slug}`;
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
