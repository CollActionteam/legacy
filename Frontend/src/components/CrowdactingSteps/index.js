import React from "react";
import Grid from "@material-ui/core/Grid";

import styles from './style.module.scss';
import { graphql, useStaticQuery } from "gatsby";

const query = graphql`
  query {
    steps: allMarkdownRemark(filter: {frontmatter: {type: {eq: "crowdactingsteps"}}}, sort: {fields: frontmatter___sequence}) {
      nodes {
        html
        frontmatter {
          name
          image
        }
      }
    }
  }
`;

export const CrowdactingSteps = () => {
  const data = useStaticQuery(query);
  const steps = data.steps.nodes;
  console.log(steps);
  
  return (
    <Grid container className={styles.main} spacing={5}>
      <Grid item xs={12}>
        <h2>Crowdacting in { steps.length } steps</h2>
      </Grid>
      { steps.map((step, index) => (
        <Grid key={index} item xs={12} md={4} className={styles.step}>
          <img src={ step.frontmatter.image }></img>
          <h1>{ step.frontmatter.name }</h1>
          <span dangerouslySetInnerHTML={{ __html: step.html }}></span>
        </Grid>
      ))}
    </Grid>
  )
};
