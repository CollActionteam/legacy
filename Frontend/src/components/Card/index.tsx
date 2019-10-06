import React from "react";
import styles from "./style.module.scss";

interface IParticipantCount {
  count: number;
}
interface ICategory {
  color: number;
  colorHex: string;
  id: number;
  name: string;
}

interface IImageFile {
  date: Date;
  description: string;
  filepath: string;
  height: number;
  id: number;
  url: string;
  width: number;
}
interface IProjectProps {
  category: ICategory;
  description: string;
  descriptiveImage: IImageFile;
  end: Date;
  goal: string;
  name: string;
  participantCounts: IParticipantCount;
  status: string;
  remainingTime: any;
  url: string;
  target: number;
}

export default ({ project }: { project: IProjectProps }) => {
  return (
    <div className={styles.card}>
      {project.descriptiveImage && (
        <figure>
          <img
            className={styles.image}
            src={project.descriptiveImage.filepath}
          ></img>
        </figure>
      )}
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
    </div>
  );
};
