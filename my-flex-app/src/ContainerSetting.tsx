

function ContainerSetting(){

    return(
        <div className="flex flex-row">
            <label htmlFor='direction' className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">Direction</label>
            <select id='direction' className='bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white focus:ring-0' value={direction} onChange={e => setDirection(e.target.value)}>
              <option value="flex-row">Row</option>
              <option value="flex-col">Column</option>
            </select>
        </div>
    )
}

export default ContainerSetting;