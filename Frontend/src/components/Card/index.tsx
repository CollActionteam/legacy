import React from "react";
import styles from "./style.module.scss";
import { IProject } from "../../api/types";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

export default ({ project }: { project: IProject }) => {
  return (
    <a href={project.url} className={styles.card}>
      <figure className={styles.image}>
        <img
          src="https://collaction-production.s3.eu-central-1.amazonaws.com/62bb2ba9-32f1-492e-99cb-95e53f71aa4a.png"
          alt={project.name}
        />
      </figure>
      {project.descriptiveImage && (
        <figure className={styles.image}>
          <img src={project.descriptiveImage.filepath} alt={project.name} />
        </figure>
      )}
      <div className={styles.content}>
        <div className={styles.statusLabel}>
          {project.isActive ? <span>Signup open</span> : null}
          {project.isComingSoon ? <span>Coming soon</span> : null}
        </div>
        <h2 className={styles.title}>
          Carbon Neutral Travel 2019
          {/* {project.title} */}
        </h2>
        <div className={styles.description}>
          If 50 people commit to (1) thinking twice about each flight they take,
          and (2) … purchasing carbon offsets ...
          {/* {project.description} */}
        </div>
        <div className={styles.stats}>
          <div className={styles.percentage}>
            {project.participantCounts ? (
              <span>
                {Math.round(project.participantCounts.count) / project.target}
              </span>
            ) : (
              <span>0</span>
            )}
            <span>%</span>
          </div>
          <div>
            <div className={styles.count}>
              {project.participantCounts ? (
                <span>{project.participantCounts.count}</span>
              ) : (
                <span>?</span>
              )}
              <span> of {project.target} participants</span>
            </div>
            <div className={styles.remainingTime}>
              <FontAwesomeIcon icon="clock"></FontAwesomeIcon>
              {project.remainingTime ? (
                <span>
                  {Math.round(project.remainingTime / 3600 / 24)} days
                </span>
              ) : (
                <span>not active</span>
              )}
            </div>
          </div>
        </div>
        <div className={styles.category}>
          Well-being
          {/* {project.category} */}
        </div>
      </div>
    </a>
  );
};
