import React from "react";
import styles from "./style.module.scss";
import { IProjectCategory } from "../../api/types";
import Utils from "../../utils";

interface ICategoryTagProperties {
  category: IProjectCategory;
}

export default ({ category }: ICategoryTagProperties) => {
  return (
    <div key={category.id} className={styles.category}>
      {Utils.formatCategory(category.name)}
    </div>
  );
};
