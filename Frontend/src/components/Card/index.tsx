import React from "react";
import styles from "./style.module.scss";
import { IProject } from "../../api/types";
import { Button } from "../Button";
import { Box } from "@material-ui/core";

export default ({ project }: { project: IProject }) => {
  return (
    <div className={styles.card}>
      {project.descriptiveImage && (
        <figure className={styles.image}>
          <img src={project.descriptiveImage.filepath} alt={project.name} />
        </figure>
      )}
      <div className={styles.content}>
        <h2 className={styles.title}>{project.name}</h2>
        <div className={styles.description}>{project.description}</div>
        <ul className={styles.stats}>
          <li>
            <span>{Math.round(project.remainingTime / 3600 / 24)} days</span>
            <span className={styles.statsLabel}>To go</span>
          </li>
          <li>
            {project.participantCounts ? (
              <div>
                <span className={styles.count}>
                  {project.participantCounts.count}
                </span>
              </div>
            ) : (
              <span>?</span>
            )}
            <span className={styles.statsLabel}>participants</span>
          </li>
          <li>
            <span>{project.target}</span>
            <span className={styles.statsLabel}>Target</span>
          </li>
        </ul>
        <Box display="flex" justifyContent="center">
          <Button to={project.url}>Read more</Button>
        </Box>
      </div>
    </div>
  );
};
