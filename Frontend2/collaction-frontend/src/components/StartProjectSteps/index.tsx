import React from "react";
import Grid from "@material-ui/core/Grid";

import styles from "./style.module.scss";

export const StartProjectSteps = () => {
  const steps = [
    {
      name: "Dummy step",
      image: "",
      html: "Dummy step html"
    }
  ];

  return (
    <Grid container className={styles.main}>
      {steps.map((step, index) => (
        <Grid key={index} item xs={12} md={4} className={styles.step}>
          <img
            alt={step.name}
            title={step.name}
            src={step.image}
          ></img>
          <h2>{step.name}</h2>
          <span dangerouslySetInnerHTML={{ __html: step.html }}></span>
        </Grid>
      ))}
    </Grid>
  );
};
