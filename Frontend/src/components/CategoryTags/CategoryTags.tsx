import React from "react";
import styles from "./CategoryTags.module.scss";
import { ICrowdactionCategory } from "../../api/types";
import Utils from "../../utils";

interface ICategoryTagProperties {
  categories: ICrowdactionCategory[];
}

const CategoryTags = ({ categories }: ICategoryTagProperties) => {
  return (
    <div>
      {categories.map((item: ICrowdactionCategory) => (
        <div key={item.category} className={styles.category}>
          {Utils.formatCategory(item.category)}
        </div>
      ))}
    </div>
  );
};

export default CategoryTags;