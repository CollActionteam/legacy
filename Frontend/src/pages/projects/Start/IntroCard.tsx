import React from "react";
import styles from "./IntroCard.module.scss";
import { SecondaryButton } from "../../../components/Button/Button";

const IntroCard = () => {
  return (
      <div className={styles.card}>
        <div className={styles.cardContent}>
          <h2>Starting a project</h2>
          <h3>(is super easy)</h3>
          <p>
            Would you like to do something about social or environmental problems? 
            Would you like to lead the crowdacting movement? Start a project on 
            CollAction now! Also, check our  <a href="https://docs.google.com/document/d/1JK058S_tZXntn3GzFYgiH3LWV5e9qQ0vXmEyV-89Tmw" target="_blank">handbook for project starters</a>.
          </p>
          <p>
            <b>It's really easy - we'll guide you through it.</b>
          </p>
        </div>
        <div className={styles.callToAction}>
          <SecondaryButton to="/projects/create">
            Start a project
          </SecondaryButton>
        </div>
      </div>
  )
}

export default IntroCard;