import React from "react";
import styles from "./CategoryTags.module.scss";
import { IProjectCategory } from "../../api/types";
import Utils from "../../utils";

interface ICategoryTagProperties {
  categories: IProjectCategory[];
}

export default ({ categories }: ICategoryTagProperties) => {
  return (
    <div>
      {categories.map((item: IProjectCategory) => (
        <div key={item.category} className={styles.category}>
          {Utils.formatCategory(item.category)}
        </div>
      ))}
    </div>
  );
};
