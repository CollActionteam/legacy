import React from "react";
import Grid from "@material-ui/core/Grid";

import ProjectIdea from "../../../assets/project-idea.png";
import ShareProject from "../../../assets/share-project.png";
import MakeWaves from "../../../assets/make-waves.png";
import { LazyLoadImage } from 'react-lazy-load-image-component';

import styles from "./StartProjectSteps.module.scss";

interface StartProjectStep {
  name: string;
  image: string;
  text: any;
}

export const StartProjectSteps = () => {
  const steps: StartProjectStep[] = [
    {
      name: "Create your project",
      image: ProjectIdea,
      text: "Complete the start project form. The CollAction team will review it as soon as possible"
    },
    {
      name: "Share your project",
      image: ShareProject,
      text: "Run a campaign to reach your target (once your project is approved)"
    },
    {
      name: "Make waves",
      image: MakeWaves,
      text: "Act together if the target is met before the deadline. And share your success and impact!"
    },
  ];

  const Step = (step: StartProjectStep, index: number) => {
    return (
      <Grid key={index} item xs={12} md={4} className={styles.step}>
          <LazyLoadImage
            alt={step.name}
            title={step.name}
            src={step.image}
            className={styles.image}
          />
          <h3>{step.name}</h3>
          <p>{step.text}</p>
      </Grid>
    );
  }

  return (
    <Grid container>
      {steps.map(Step)}
    </Grid>
  );
};
