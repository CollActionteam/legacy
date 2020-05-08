import React from "react";
import Grid from "@material-ui/core/Grid";

import CrowdactionIdea from "../../../assets/crowdaction-idea.png";
import ShareCrowdaction from "../../../assets/share-crowdaction.png";
import MakeWaves from "../../../assets/make-waves.png";
import LazyImage from "../../../components/LazyImage/LazyImage";

import styles from "./StartCrowdactionSteps.module.scss";

interface StartCrowdactionStep {
  name: string;
  image: string;
  text: any;
}

export const StartCrowdactionSteps = () => {
  const steps: StartCrowdactionStep[] = [
    {
      name: "Create your crowdaction",
      image: CrowdactionIdea,
      text: "Complete the start crowdaction form. The CollAction team will review it as soon as possible"
    },
    {
      name: "Share your crowdaction",
      image: ShareCrowdaction,
      text: "Run a campaign to reach your target (once your crowdaction is approved)"
    },
    {
      name: "Make waves",
      image: MakeWaves,
      text: "Act together if the target is met before the deadline. And share your success and impact!"
    },
  ];

  const Step = (step: StartCrowdactionStep, index: number) => {
    return (
      <Grid key={index} item xs={12} md={4} className={styles.step}>
          <LazyImage
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
