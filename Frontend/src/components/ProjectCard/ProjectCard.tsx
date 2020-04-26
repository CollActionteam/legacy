import React from "react";
import styles from "./style.module.scss";
import { IProject } from "../../api/types";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import ProgressRing from "../ProgressRing/ProgressRing";
import CategoryTags from "../CategoryTags/CategoryTags";

export default ({ project }: { project: IProject }) => {
  const defaultCategoryImage = project.categories[0]
    ? project.categories[0].category
    : "OTHER";

  const defaultBanner = require(`../../assets/default_banners/${defaultCategoryImage}.jpg`);

  return (
    <a href={project.url} className={styles.card}>
      <figure className={styles.image}>
        { project.cardImage ? <img src={project.cardImage.url} alt={project.cardImage.description} /> : <img src={defaultBanner} alt={project.name} /> }
      </figure>
      <div className={styles.content}>
        <div className={styles.statusLabel}>
          {project.isActive ? <span>Signup open</span> : null}
          {project.isComingSoon ? <span>Coming soon</span> : null}
          {project.isClosed ? <span>Signup closed</span> : null}
        </div>
        <h3 className={styles.title}>{project.name}</h3>
        <div className={styles.proposal}>{project.proposal}</div>
        <div className={styles.stats}>
          <div className={styles.percentage}>
            <ProgressRing progress={project.percentage} />
          </div>
          <div>
            <div className={styles.count}>
              <span>{project.totalParticipants}</span>
              <span> of {project.target} participants</span>
            </div>
            <div className={styles.remainingTime}>
              <FontAwesomeIcon icon="clock"></FontAwesomeIcon>
              <span>
                { project.remainingTimeUserFriendly }
              </span>
            </div>
          </div>
        </div>
        <CategoryTags categories={project.categories}></CategoryTags>
      </div>
    </a>
  );
};
