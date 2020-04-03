import React from 'react';
import { FormControl, InputLabel, MenuItem, FormHelperText } from '@material-ui/core';
import { Select } from 'formik-material-ui';
import styles from './Create.module.scss';
import { Field } from 'formik';
import { useQuery, gql } from '@apollo/client';

import Utils from '../../../utils';

const Categories = (formik: any) => {

  const { data: categoryResponse } = useQuery(gql`
    query {
      __type(name: "Category") {
        enumValues {
          name
          description
        }
      }
    }
  `);  
  const categories = categoryResponse 
    ? categoryResponse.__type.enumValues.map((c: any) => Utils.formatCategory(c.name))
    : [];

  return (
    <FormControl fullWidth className={styles.formRow}>
      <InputLabel htmlFor="category">Category</InputLabel>
      <Field 
        name="category" 
        component={Select} 
        error={formik.props.touched.category && Boolean(formik.props.errors.category)}
      >
        {categories && categories.map((category: string) => (
          <MenuItem key={category} value={category}>
            {category}
          </MenuItem>
        ))}
      </Field>
      <FormHelperText error={formik.props.touched.category && Boolean(formik.props.errors.category)}>
        {formik.props.errors.category}
      </FormHelperText>
      <FormHelperText hidden={formik.props.touched.category && Boolean(formik.props.errors.category)}>
        Choose the category that most closely aligns with your crowdaction
      </FormHelperText>
    </FormControl>
  );
}

export default Categories;
