import * as React from 'react'
import { Grid, IGridHeader, IGridColumn, _, gridUtil } from '@zen-components/grid/gridExports'
import * as paramsParser from '@zen-components/util/paramsParser'
import * as mpxTableUtil from './mpxTableUtil'
import * as inputTableHelper from './inputTableHelper'
import * as inputProductsRoutingsHeaders from './inputProductsRoutingsHeaders'

import { GridTitleBar } from './GridTitleBar'
import { DialogError } from './DialogError'
import { ProductFilter } from './ProductFilter'

import { config } from './../config'

const apiName = 'products-routings'
//const tableName = 'tbloper'
const idColumn = 'recid'

const hiddenRows = ['DOCK', 'STOCK', 'SCRAP']

export class InputProductsRoutings extends React.Component<{}, {}> {
  state = {
    showAdvanced: false,
    headers: [] as IGridHeader[],
    data: [],
    selectedRows: [],
    selectedProductName: '',
    allProductNames: [],
    hasError: false,
    error: null as string,
  }

  componentWillMount() {
    let selectedProductName = paramsParser.getUrlParameter('product') || ''
    let headers = inputProductsRoutingsHeaders.buildHeaders(this.state.showAdvanced)
    this.setState({ headers, selectedProductName })
    this._fetch()
  }

  _fetch = async () => {

    let allProductNames = await mpxTableUtil.selectList('products', 'proddesc')
    this.setState({ allProductNames: allProductNames })

    let data = await mpxTableUtil.fetchTableJson(apiName)
    // Filter out rows
    data = _.filter(data, (c) => !_.some(hiddenRows, (d) => c.opnam === d))
    // data = _.filter(data, c => c.opnam !== 'STOCK')
    this.setState({ data })
  }

  _onUpdateCell = async (newValue: any, column: IGridColumn, dataRow: any) => {
    await inputTableHelper.updateRow(this, apiName, idColumn, dataRow[idColumn], column.header.dataItem, newValue)
  }

  _onToggleShowAdvanced = () => {
    console.log('toggle show advanced')
    let showAdvanced = !this.state.showAdvanced
    let headers = inputProductsRoutingsHeaders.buildHeaders(showAdvanced)
    this.setState({ showAdvanced, headers })
  }

  _onDeleteSelectedRows = async () => {
    await inputTableHelper.deleteSelectedRows(this, apiName, idColumn)
  }

  _onAddRow = async () => {
    await inputTableHelper.addNewRow(this, apiName, idColumn)
  }

  _onSelectRow = (dataRow, isSelected, mode) => {
    inputTableHelper.selectRow(this, dataRow, isSelected, mode)
  }

  _onChange_selectedProductName = (newSelectedProductName) => {
    this.setState({ selectedProductName: newSelectedProductName })
  }

  _onBack = () => {
    config.navigateTo('/input/products.aspx?mode=products')
  }

  _onClose_dialogError = () => {
    this.setState({ hasError: false })
  }

  render() {

    let filteredData = _.filter(this.state.data, (c) => c.proddesc === this.state.selectedProductName)

    return (
      <div>
        <GridTitleBar
          title='Products - Routings'
          showAdvanced={this.state.showAdvanced}
          onToggleShowAdvanced={this._onToggleShowAdvanced}
          onBack={this._onBack}
          onAdd={this._onAddRow}
          onDeleteSelected={this._onDeleteSelectedRows}
          hasSelected={this.state.selectedRows.length > 0}
        />
        <ProductFilter
          allProductNames={this.state.allProductNames}
          selectedProductName={this.state.selectedProductName}
          onChange={this._onChange_selectedProductName}
        />
        <Grid
          headers={this.state.headers}
          data={filteredData}
          onUpdateCell={this._onUpdateCell}
          selectedRows={this.state.selectedRows}
          onSelectRow={this._onSelectRow}
        />
        <DialogError isOpen={this.state.hasError} error={this.state.error} onClose={this._onClose_dialogError} />
      </div>
    )
  }
}
