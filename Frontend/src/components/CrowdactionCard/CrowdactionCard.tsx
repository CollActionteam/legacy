import React from "react";
import styles from "./CrowdactionCard.module.scss";
import { ICrowdaction } from "../../api/types";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import ProgressRing from "../ProgressRing/ProgressRing";
import CategoryTags from "../CategoryTags/CategoryTags";
import Formatter from "../../formatter";
import LazyImage from "../LazyImage/LazyImage";

interface ICrowdactionCardProps {
  crowdaction: ICrowdaction;
  target?: string | undefined;
}

const CrowdactionCard = ({ crowdaction, target }: ICrowdactionCardProps) => {
  const defaultCategoryImage = crowdaction.categories[0]
    ? crowdaction.categories[0].category
    : "OTHER";

  const defaultCardImage = require(`../../assets/default_card_images/${defaultCategoryImage}.jpg`);

  return (
    <a href={crowdaction.url} className={styles.card} target={target ?? "_self"}>
      <figure className={styles.image}>
        { crowdaction.cardImage ? <LazyImage src={crowdaction.cardImage.url} alt={crowdaction.cardImage.description} /> : <LazyImage src={defaultCardImage} alt={crowdaction.name} /> }
      </figure>
      <div className={styles.content}>
        <div className={styles.statusLabel}>
          {crowdaction.isActive ? <span>Signup open</span> : null}
          {crowdaction.isComingSoon ? <span>Coming soon</span> : null}
          {crowdaction.isClosed ? <span>Signup closed</span> : null}
        </div>
        <h3 className={styles.title}>{crowdaction.name}</h3>
        <div className={styles.proposal}>{crowdaction.proposal}</div>
        <div className={styles.stats}>
          <div className={styles.percentage}>
            <ProgressRing progress={crowdaction.percentage} />
          </div>
          <div>
            <div className={styles.count}>
              <span>{Formatter.largeNumber(crowdaction.totalParticipants)}</span>
              <span> of {Formatter.largeNumber(crowdaction.target)} participants</span>
            </div>
            <div className={styles.remainingTime}>
              <FontAwesomeIcon icon="clock"></FontAwesomeIcon>
              <span>
                { crowdaction.remainingTimeUserFriendly }
              </span>
            </div>
          </div>
        </div>
        <CategoryTags categories={crowdaction.categories}></CategoryTags>
      </div>
    </a>
  );
};

export default CrowdactionCard;